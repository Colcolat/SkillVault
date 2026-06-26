using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Ports.Input;

namespace SkillVault.Controllers;

/// <summary>
/// Manages progress-tracking operations in SkillVault.
/// 
/// This is the core operation of the application: registering study hours
/// against a certification, as documented in ADR-02's Process View.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ProgressController : ControllerBase
{
    private readonly IProgressUseCase _progressUseCase;
    private readonly ILogger<ProgressController> _logger;

    public ProgressController(IProgressUseCase progressUseCase, ILogger<ProgressController> logger)
    {
        _progressUseCase = progressUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves the overall progress summary across all certifications.
    /// </summary>
    /// <response code="200">Successfully retrieved progress summary</response>
    [HttpGet]
    [ProducesResponseType(typeof(ProgressSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProgressSummaryDto>> GetProgressSummary()
    {
        var summary = await _progressUseCase.GetProgressSummaryAsync();
        return Ok(summary);
    }

    /// <summary>
    /// Retrieves all progress entries for a specific certification.
    /// </summary>
    /// <param name="certificationId">The ID of the certification</param>
    [HttpGet("certification/{certificationId}")]
    [ProducesResponseType(typeof(IEnumerable<ProgressDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProgressDto>>> GetProgressByCertification(int certificationId)
    {
        var progress = await _progressUseCase.GetProgressByCertificationAsync(certificationId);
        return Ok(progress);
    }

    /// <summary>
    /// Registers study hours against a certification.
    /// 
    /// This endpoint implements the Process View documented in ADR-02:
    /// validates the certification exists, validates hours, persists the entry.
    /// </summary>
    /// <param name="request">Certification ID, hours spent, and optional notes</param>
    /// <response code="201">Progress successfully recorded</response>
    /// <response code="400">Validation failed or certification not found</response>
    /// <example>
    /// POST /api/v1/progress
    /// {
    ///   "certificationId": 1,
    ///   "hours": 2.5,
    ///   "notes": "Completed AWS EC2 module"
    /// }
    /// </example>
    [HttpPost]
    [ProducesResponseType(typeof(ProgressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProgressDto>> RegisterProgress([FromBody] CreateProgressRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var progress = await _progressUseCase.RegisterProgressAsync(request);
            return CreatedAtAction(nameof(GetProgressByCertification),
                new { certificationId = progress.CertificationId }, progress);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering progress");
            return StatusCode(500, new { message = "An error occurred while registering progress" });
        }
    }
}
