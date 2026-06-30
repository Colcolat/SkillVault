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

    /// <summary>
    /// Registers a new user dynamically in memory.
    /// </summary>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Invalid request or user already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Register([FromBody] LoginRequest request)
    {
        try
        {
            var success = await _authUseCase.RegisterAsync(request);
            if (!success)
                return BadRequest(new { message = "User already exists" });

            return Ok(new { message = "User registered successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration process");
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }
}
