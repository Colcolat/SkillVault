using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

/// <summary>
/// Represents a certification in API responses.
/// </summary>
public class CertificationDto
{
    public int Id { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime CompletedDate { get; set; }
    public string? CredentialUrl { get; set; }
    public IEnumerable<int> SkillIds { get; set; } = Array.Empty<int>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Request body for creating a new certification.
/// </summary>
public class CreateCertificationRequest
{
    /// <summary>
    /// The organization that issued the certification (e.g., "Amazon", "HackerRank").
    /// </summary>
    [Required(ErrorMessage = "Provider is required")]
    [MaxLength(100)]
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// The title of the certification.
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The date when the certification was completed. Cannot be in the future.
    /// </summary>
    [Required(ErrorMessage = "CompletedDate is required")]
    public DateTime CompletedDate { get; set; }

    /// <summary>
    /// Optional URL to verify the credential online.
    /// </summary>
    public string? CredentialUrl { get; set; }

    /// <summary>
    /// IDs of skills to associate with this certification.
    /// </summary>
    public IEnumerable<int> SkillIds { get; set; } = Array.Empty<int>();
}

/// <summary>
/// Request body for updating an existing certification.
/// </summary>
public class UpdateCertificationRequest
{
    [MaxLength(200)]
    public string? Title { get; set; }

    public string? CredentialUrl { get; set; }

    public IEnumerable<int>? SkillIds { get; set; }
}
