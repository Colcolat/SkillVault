using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProgressController : ControllerBase
    {
        // GET: api/v1/progress
        [HttpGet]
        public IActionResult GetSummary()
        {
            var summary = new
            {
                TotalHours = 120,
                ActiveCourses = 3,
                CompletedCertifications = 2
            };
            return Ok(summary);
        }

        // POST: api/v1/progress
        [HttpPost]
        public IActionResult RegisterStudyHours([FromBody] object progressData)
        {
            return Ok(new { Message = "Study hours registered successfully", ProgressPercentage = 85 });
        }
    }
}