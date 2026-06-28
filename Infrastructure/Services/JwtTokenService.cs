using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Ports.Output;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

/// <summary>
/// Generates and validates JWT tokens for SkillVault API authentication.
/// Implements IJwtTokenGenerator to satisfy Hexagonal architectural boundaries.
/// </summary>
public class JwtTokenService : IJwtTokenGenerator
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtTokenService(IConfiguration configuration)
    {
        // Read configuration from "Jwt" section of appsettings.json
        var jwtSection = configuration.GetSection("Jwt");
        _secretKey = jwtSection["SecretKey"] ?? "your-super-secret-jwt-key-that-is-at-least-32-characters-long";
        _issuer = jwtSection["Issuer"] ?? "SkillVault";
        _audience = jwtSection["Audience"] ?? "SkillVaultAPI";
        
        var expirationString = jwtSection["ExpirationMinutes"];
        _expirationMinutes = int.TryParse(expirationString, out var result) ? result : 1440;
    }

    /// <summary>
    /// Generates a JWT token for the given user email.
    /// </summary>
    public string GenerateToken(string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.NameIdentifier, email),
            new Claim("user_id", email) // Custom claim for the app
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Validates a JWT token and returns claims principal if valid.
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
