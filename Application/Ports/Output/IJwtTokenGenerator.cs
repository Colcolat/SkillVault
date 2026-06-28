namespace Application.Ports.Output;

/// <summary>
/// Output port for generating authentication tokens.
/// Implemented by Infrastructure adapters to keep the business core isolated.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a token representing the authenticated user.
    /// </summary>
    string GenerateToken(string email);
}
