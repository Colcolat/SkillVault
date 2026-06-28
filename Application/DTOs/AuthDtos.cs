namespace Application.DTOs;

/// <summary>
/// DTO representing the login request payload.
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO representing the response containing the generated JWT token.
/// </summary>
public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; } // seconds
    public string TokenType { get; set; } = "Bearer";
}
