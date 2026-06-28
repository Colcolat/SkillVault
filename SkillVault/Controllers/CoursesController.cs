using Application.DTOs;
using Application.Ports.Input;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SkillVault.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseUseCase _courseUseCase;

    public CoursesController(ICourseUseCase courseUseCase)
    {
        _courseUseCase = courseUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll()
    {
        var courses = await _courseUseCase.GetAllCoursesAsync();
        return Ok(courses);
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _courseUseCase.AddCourseAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateCourseRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _courseUseCase.UpdateCourseStatusAsync(id, request.Status);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _courseUseCase.DeleteCourseAsync(id);
        return NoContent();
    }
}
