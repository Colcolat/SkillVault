using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SkillsController : ControllerBase
    {
        // GET: api/v1/skills
        [HttpGet]
        public IActionResult GetAll()
        {
            var skills = new[]
            {
                new { Id = 1, Name = "C#", Category = "Backend" },
                new { Id = 2, Name = "AWS EC2", Category = "Cloud" }
            };
            return Ok(skills);
        }

        // POST: api/v1/skills
        [HttpPost]
        public IActionResult Create([FromBody] object newSkill)
        {
            return StatusCode(201, new { Message = "Skill created in the catalog" });
        }
    }
}