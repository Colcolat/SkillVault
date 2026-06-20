using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CertificationsController : ControllerBase
    {
        // GET: api/v1/certifications
        [HttpGet]
        public IActionResult GetAll()
        {
            // Placeholder: Returns a static list
            var certs = new[]
            {
                new { Id = 1, Title = "AWS Certified Cloud Practitioner", Provider = "Amazon", CompletedDate = "2026-05-19" },
                new { Id = 2, Title = "Azure Fundamentals", Provider = "Microsoft", CompletedDate = "2026-06-01" }
            };
            return Ok(certs);
        }

        // GET: api/v1/certifications/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // Placeholder
            return Ok(new { Id = id, Title = "Example Certification", Provider = "Example" });
        }

        // POST: api/v1/certifications
        [HttpPost]
        public IActionResult Create([FromBody] object newCertification)
        {
            // Placeholder: Simulates a successful creation
            return CreatedAtAction(nameof(GetById), new { id = 3 }, new { Message = "Certification registered successfully" });
        }

        // PUT: api/v1/certifications/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] object updatedCertification)
        {
            return Ok(new { Message = $"Certification {id} updated successfully" });
        }

        // DELETE: api/v1/certifications/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return NoContent(); // 204 No Content = successful deletions
        }
    }
}