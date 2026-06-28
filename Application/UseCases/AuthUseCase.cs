using Application.DTOs;
using Application.Ports.Input;
using Application.Ports.Output;

namespace Application.UseCases;

/// <summary>
/// Implements authentication use cases.
/// Decoupled from infrastructure using IJwtTokenGenerator output port.
/// </summary>
public class AuthUseCase : IAuthUseCase
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    // Hardcoded credentials for exposition/milestone purposes
    private const string ValidEmail = "jj@skillvault.dev";
    private const string ValidPassword = "accenture2026";

    public AuthUseCase(IJwtTokenGenerator jwtTokenGenerator)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <summary>
    /// Processes user login. Compares with hardcoded credentials and generates a JWT.
    /// </summary>
    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        // Simple validation
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Email and password are required");

        // Verify credentials
        if (request.Email != ValidEmail || request.Password != ValidPassword)
            throw new UnauthorizedAccessException("Invalid email or password");

        // Generate token using the output port adapter
        var token = _jwtTokenGenerator.GenerateToken(request.Email);

        return new TokenResponse
        {
            AccessToken = token,
            ExpiresIn = 24 * 60 * 60, // 24 hours in seconds
            TokenType = "Bearer"
        };
    }
}
