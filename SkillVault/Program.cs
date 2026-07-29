using Microsoft.EntityFrameworkCore;
using Application.Ports.Input;
using Application.Ports.Output;
using Application.UseCases;
using Infrastructure.Adapters.Output;
using Infrastructure.Persistence;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────
// Database (PostgreSQL via EF Core)
// ─────────────────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Hack para Render.com: Convierte la URL 'postgres://user:pass@host/db' al formato de ADO.NET
if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgres"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':', 2);
    var username = Uri.UnescapeDataString(userInfo[0]);
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
    connectionString = $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};Database={uri.LocalPath.Substring(1)};Username={username};Password={password};SslMode=Require;Trust Server Certificate=true;";
}

builder.Services.AddDbContext<SkillVaultDbContext>(options =>
    options.UseNpgsql(connectionString));

// ─────────────────────────────────────────────────────────────────────────
// Dependency Injection — Hexagonal Architecture wiring
// Application layer depends on interfaces (Ports).
// Here is the ONLY place that decides which concrete Adapter is used.
// ─────────────────────────────────────────────────────────────────────────

// Output Ports → Infrastructure Adapters (production = PostgreSQL)
builder.Services.AddScoped<ICertificationRepository, PostgreSQLCertificationRepository>();
builder.Services.AddScoped<ISkillRepository, PostgreSQLSkillRepository>();
builder.Services.AddScoped<IProgressRepository, PostgreSQLProgressRepository>();
builder.Services.AddScoped<ICourseRepository, PostgreSQLCourseRepository>();

// Input Ports → Application UseCases
builder.Services.AddScoped<ICertificationUseCase, CertificationUseCase>();
builder.Services.AddScoped<ISkillUseCase, SkillUseCase>();
builder.Services.AddScoped<IProgressUseCase, ProgressUseCase>();
builder.Services.AddScoped<ICourseUseCase, CourseUseCase>();

// JWT Authentication & Authorization setup
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? "your-super-secret-jwt-key-that-is-at-least-32-characters-long";

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = null; // No external identity authority
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "SkillVault",
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"] ?? "SkillVaultAPI",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Register Authentication Services
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenService>();
builder.Services.AddScoped<JwtTokenService>(); // Also register concrete class
builder.Services.AddScoped<IAuthUseCase, AuthUseCase>();

// ─────────────────────────────────────────────────────────────────────────
// Controllers
// ─────────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ─────────────────────────────────────────────────────────────────────────
// Swagger / OpenAPI
// ─────────────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// ─────────────────────────────────────────────────────────────────────────
// CORS (for future frontend client)
// ─────────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Fail fast if the database is unreachable or the schema is out of date.
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SkillVaultDbContext>();
    
    // Add retry loop for Render DNS propagation
    int maxRetries = 5;
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            await dbContext.Database.MigrateAsync();
            break;
        }
        catch (Exception ex)
        {
            if (i == maxRetries - 1) throw;
            Console.WriteLine($"Database connection failed: {ex.Message}. Retrying in 5 seconds...");
            await Task.Delay(5000);
        }
    }
}

// ─────────────────────────────────────────────────────────────────────────
// HTTP request pipeline
// ─────────────────────────────────────────────────────────────────────────

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillVault API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at root: http://localhost:5000
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check (no auth required) — useful for AWS load balancer checks later
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow })
   .WithName("HealthCheck")
   .WithOpenApi()
   .Produces(200);

app.MapGet("/health/db", async (SkillVaultDbContext dbContext) =>
{
    var canConnect = await dbContext.Database.CanConnectAsync();
    return canConnect
        ? Results.Ok(new { status = "healthy", database = "connected", timestamp = DateTime.UtcNow })
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
});

app.Run();
