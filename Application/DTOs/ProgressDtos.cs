using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

/// <summary>
/// Represents a single progress entry in API responses.
/// </summary>
public class ProgressDto
{
    public int Id { get; set; }
    public int CertificationId { get; set; }
    public int? SkillId { get; set; }
    public decimal Hours { get; set; }
    public string? Notes { get; set; }
    public DateTime RecordedAt { get; set; }
}

/// <summary>
/// Request body for registering a new progress entry.
/// 
/// This is the core operation documented in the Process View (ADR-02):
/// POST /api/v1/progress -> validates -> updates accumulated progress -> persists.
/// </summary>
public class CreateProgressRequest
{
    [Required(ErrorMessage = "CertificationId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "CertificationId must be a valid ID")]
    public int CertificationId { get; set; }

    /// <summary>
    /// Optional: associates this progress entry with a specific skill,
    /// used by the Strategy pattern to calculate skill-level completion.
    /// </summary>
    public int? SkillId { get; set; }

    [Required(ErrorMessage = "Hours is required")]
    [Range(0.1, 24, ErrorMessage = "Hours must be between 0.1 and 24")]
    public decimal Hours { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

/// <summary>
/// Aggregated progress summary across all certifications, returned by
/// GET /api/v1/progress.
/// </summary>
public class ProgressSummaryDto
{
    public int TotalCertifications { get; set; }
    public int CompletedCertifications { get; set; }
    public int InProgressCertifications { get; set; }
    public decimal TotalHoursSpent { get; set; }
    public IEnumerable<CertificationProgressItem> CertificationProgress { get; set; } = Array.Empty<CertificationProgressItem>();
}

/// <summary>
/// Per-certification progress breakdown used inside ProgressSummaryDto.
/// </summary>
public class CertificationProgressItem
{
    public int CertificationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal HoursSpent { get; set; }
}