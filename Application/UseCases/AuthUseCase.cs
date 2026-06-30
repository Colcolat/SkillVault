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

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> RegisteredUsers = new(
        new[] { new System.Collections.Generic.KeyValuePair<string, string>("jj@skillvault.dev", "accenture2026") }
    );

    public AuthUseCase(IJwtTokenGenerator jwtTokenGenerator)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <summary>
    /// Processes user login. Compares with stored credentials and generates a JWT.
    /// </summary>
    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        // Simple validation
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Email and password are required");

        // Verify credentials in memory
        if (!RegisteredUsers.TryGetValue(request.Email, out var password) || password != request.Password)
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

    /// <summary>
    /// Registers a new user dynamically in memory.
    /// </summary>
    public async Task<bool> RegisterAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Email and password are required");

        return RegisteredUsers.TryAdd(request.Email, request.Password);
    }
}
