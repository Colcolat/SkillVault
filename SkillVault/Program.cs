using Microsoft.EntityFrameworkCore;
using Application.Ports.Input;
using Application.Ports.Output;
using Application.UseCases;
using Infrastructure.Adapters.Output;
using Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────
// Database (PostgreSQL via EF Core)
// ─────────────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<SkillVaultDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─────────────────────────────────────────────────────────────────────────
// Dependency Injection — Hexagonal Architecture wiring
// Application layer depends on interfaces (Ports).
// Here is the ONLY place that decides which concrete Adapter is used.
// ─────────────────────────────────────────────────────────────────────────

// Output Ports → Infrastructure Adapters (production = PostgreSQL)
builder.Services.AddScoped<ICertificationRepository, PostgreSQLCertificationRepository>();
builder.Services.AddScoped<ISkillRepository, PostgreSQLSkillRepository>();
builder.Services.AddScoped<IProgressRepository, PostgreSQLProgressRepository>();

// Input Ports → Application UseCases
builder.Services.AddScoped<ICertificationUseCase, CertificationUseCase>();
builder.Services.AddScoped<ISkillUseCase, SkillUseCase>();
builder.Services.AddScoped<IProgressUseCase, ProgressUseCase>();

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
app.UseAuthorization();

app.MapControllers();

// Health check (no auth required) — useful for AWS load balancer checks later
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow })
   .WithName("HealthCheck")
   .WithOpenApi()
   .Produces(200);

app.Run();
