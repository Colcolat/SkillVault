using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.DTOs;
using Application.Ports.Input;

namespace SkillVault.Controllers;

/// <summary>
/// Manages skill category operations in SkillVault.
/// Skills group related certifications (e.g., "Cloud Computing" groups AWS, Azure, GCP certs).
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class SkillsController : ControllerBase
{
    private readonly ISkillUseCase _skillUseCase;
    private readonly ILogger<SkillsController> _logger;

    public SkillsController(ISkillUseCase skillUseCase, ILogger<SkillsController> logger)
    {
        _skillUseCase = skillUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all skill categories.
    /// </summary>
    /// <response code="200">Successfully retrieved skills</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SkillDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SkillDto>>> GetAllSkills()
    {
        var skills = await _skillUseCase.GetAllSkillsAsync();
        return Ok(skills);
    }

    /// <summary>
    /// Retrieves a specific skill by ID.
    /// </summary>
    /// <param name="id">The ID of the skill to retrieve</param>
    /// <response code="200">Successfully retrieved the skill</response>
    /// <response code="404">Skill not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SkillDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SkillDto>> GetSkillById(int id)
    {
        var skill = await _skillUseCase.GetSkillByIdAsync(id);

        if (skill == null)
            return NotFound(new { message = $"Skill with ID {id} not found" });

        return Ok(skill);
    }

    /// <summary>
    /// Creates a new skill category.
    /// </summary>
    /// <param name="request">Skill data: name, description, level, target hours</param>
    /// <response code="201">Skill successfully created</response>
    /// <response code="400">Validation failed</response>
    /// <example>
    /// POST /api/v1/skills
    /// {
    ///   "name": "Cloud Computing",
    ///   "description": "AWS, Azure, GCP fundamentals and services",
    ///   "level": "Intermediate",
    ///   "targetHours": 150
    /// }
    /// </example>
    [HttpPost]
    [ProducesResponseType(typeof(SkillDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SkillDto>> CreateSkill([FromBody] CreateSkillRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var skill = await _skillUseCase.CreateSkillAsync(request);
            return CreatedAtAction(nameof(GetSkillById), new { id = skill.Id }, skill);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating skill");
            return StatusCode(500, new { message = "An error occurred while creating the skill" });
        }
    }

    /// <summary>
    /// Updates an existing skill.
    /// </summary>
    /// <param name="id">The ID of the skill to update</param>
    /// <param name="request">Fields to update</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SkillDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SkillDto>> UpdateSkill(int id, [FromBody] UpdateSkillRequest request)
    {
        var skill = await _skillUseCase.UpdateSkillAsync(id, request);

        if (skill == null)
            return NotFound(new { message = $"Skill with ID {id} not found" });

        return Ok(skill);
    }

    /// <summary>
    /// Deletes a skill by ID. This operation is permanent.
    /// </summary>
    /// <param name="id">The ID of the skill to delete</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteSkill(int id)
    {
        var success = await _skillUseCase.DeleteSkillAsync(id);

        if (!success)
            return NotFound(new { message = $"Skill with ID {id} not found" });

        return NoContent();
    }
}
