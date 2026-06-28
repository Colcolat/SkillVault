using Application.DTOs;
using Application.Ports.Input;
using Microsoft.AspNetCore.Mvc;

namespace SkillVault.Controllers;

/// <summary>
/// Handles authentication requests, issuing JWT tokens for valid credentials.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthUseCase _authUseCase;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthUseCase authUseCase, ILogger<AuthController> logger)
    {
        _authUseCase = authUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// Demo credentials: email: jj@skillvault.dev, password: accenture2026
    /// </summary>
    /// <response code="200">Token generated successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Invalid login credentials</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authUseCase.LoginAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login process");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }
}
