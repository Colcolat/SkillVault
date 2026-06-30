using Application.DTOs;

namespace Application.Ports.Input;

/// <summary>
/// Input port for authentication use cases.
/// </summary>
public interface IAuthUseCase
{
    /// <summary>
    /// Processes a login request and returns an access token if valid.
    /// </summary>
    Task<TokenResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// Registers a new user in memory for exposition purposes.
    /// </summary>
    Task<bool> RegisterAsync(LoginRequest request);
}
