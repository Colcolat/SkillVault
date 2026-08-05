using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SkillVault.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize] // Requires login to ask for tips
public class CoachController : ControllerBase
{
    private readonly ICoachService _coachService;
    private readonly ILogger<CoachController> _logger;

    public CoachController(ICoachService coachService, ILogger<CoachController> logger)
    {
        _coachService = coachService;
        _logger = logger;
    }

    [HttpGet("tips")]
    public async Task<ActionResult<object>> GetTips([FromQuery] string? title, [FromQuery] string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
            return BadRequest(new { message = "Course title is required." });

        try
        {
            var tips = await _coachService.GetStudyTipsAsync(title, description ?? string.Empty);
            return Ok(new { tips });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tips from Gemini");
            return StatusCode(500, new { message = "Failed to get tips from AI Coach." });
        }
    }
}
