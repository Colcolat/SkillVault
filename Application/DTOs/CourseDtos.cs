using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Url { get; set; }
    public decimal TotalHours { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCourseRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Provider { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Url { get; set; }
}

public class UpdateCourseRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
