using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Ports.Input;

namespace SkillVault.Controllers;

/// <summary>
/// Manages certification-related operations in SkillVault.
/// 
/// Certifications represent completed courses, exams, and learning achievements
/// from platforms like AWS, HackerRank, Google, Cisco, etc.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CertificationsController : ControllerBase
{
    private readonly ICertificationUseCase _certificationUseCase;
    private readonly ILogger<CertificationsController> _logger;

    public CertificationsController(
        ICertificationUseCase certificationUseCase,
        ILogger<CertificationsController> logger)
    {
        _certificationUseCase = certificationUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all certifications.
    /// </summary>
    /// <response code="200">Successfully retrieved certifications</response>
    /// <response code="500">An error occurred while retrieving certifications</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CertificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CertificationDto>>> GetAllCertifications()
    {
        try
        {
            var certifications = await _certificationUseCase.GetAllCertificationsAsync();
            return Ok(certifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving certifications");
            return StatusCode(500, new { message = "An error occurred while retrieving certifications" });
        }
    }

    /// <summary>
    /// Retrieves a specific certification by ID.
    /// </summary>
    /// <param name="id">The ID of the certification to retrieve</param>
    /// <response code="200">Successfully retrieved the certification</response>
    /// <response code="404">Certification not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CertificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CertificationDto>> GetCertificationById(int id)
    {
        var certification = await _certificationUseCase.GetCertificationByIdAsync(id);

        if (certification == null)
            return NotFound(new { message = $"Certification with ID {id} not found" });

        return Ok(certification);
    }

    /// <summary>
    /// Retrieves certifications filtered by skill category.
    /// </summary>
    /// <param name="skillId">The ID of the skill to filter by</param>
    [HttpGet("skill/{skillId}")]
    [ProducesResponseType(typeof(IEnumerable<CertificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CertificationDto>>> GetCertificationsBySkill(int skillId)
    {
        var certifications = await _certificationUseCase.GetCertificationsBySkillAsync(skillId);
        return Ok(certifications);
    }

    /// <summary>
    /// Registers a new certification.
    /// </summary>
    /// <param name="request">Certification data: provider, title, completion date</param>
    /// <response code="201">Certification successfully created</response>
    /// <response code="400">Validation failed</response>
    /// <example>
    /// POST /api/v1/certifications
    /// {
    ///   "provider": "Amazon",
    ///   "title": "AWS Certified Cloud Practitioner",
    ///   "completedDate": "2026-05-19",
    ///   "credentialUrl": "https://aws.amazon.com/verification"
    /// }
    /// </example>
    [HttpPost]
    [ProducesResponseType(typeof(CertificationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CertificationDto>> CreateCertification([FromBody] CreateCertificationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var certification = await _certificationUseCase.RegisterCertificationAsync(request);
            return CreatedAtAction(nameof(GetCertificationById), new { id = certification.Id }, certification);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating certification");
            return StatusCode(500, new { message = "An error occurred while creating the certification" });
        }
    }

    /// <summary>
    /// Updates an existing certification.
    /// </summary>
    /// <param name="id">The ID of the certification to update</param>
    /// <param name="request">Fields to update</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CertificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CertificationDto>> UpdateCertification(int id, [FromBody] UpdateCertificationRequest request)
    {
        var certification = await _certificationUseCase.UpdateCertificationAsync(id, request);

        if (certification == null)
            return NotFound(new { message = $"Certification with ID {id} not found" });

        return Ok(certification);
    }

    /// <summary>
    /// Deletes a certification by ID. This operation is permanent.
    /// </summary>
    /// <param name="id">The ID of the certification to delete</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteCertification(int id)
    {
        var success = await _certificationUseCase.DeleteCertificationAsync(id);

        if (!success)
            return NotFound(new { message = $"Certification with ID {id} not found" });

        return NoContent();
    }
}
