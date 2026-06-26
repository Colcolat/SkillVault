using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

/// <summary>
/// Represents a skill in API responses.
/// </summary>
public class SkillDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int TargetHours { get; set; }
    public int CertificationCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Request body for creating a new skill.
/// </summary>
public class CreateSkillRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// One of: Beginner, Intermediate, Advanced, Expert.
    /// </summary>
    public string Level { get; set; } = "Beginner";

    [Range(1, 10000, ErrorMessage = "TargetHours must be between 1 and 10000")]
    public int TargetHours { get; set; } = 100;
}

/// <summary>
/// Request body for updating an existing skill.
/// </summary>
public class UpdateSkillRequest
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public string? Level { get; set; }

    [Range(1, 10000)]
    public int? TargetHours { get; set; }
}
