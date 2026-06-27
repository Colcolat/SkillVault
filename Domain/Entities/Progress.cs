namespace Domain.Entities;

/// <summary>
/// Represents a recorded study/progress entry for a certification and optional skill.
/// </summary>
public class Progress
{
    public int Id { get; set; }
    public int CertificationId { get; set; }
    public Certification? Certification { get; set; }
    public int? SkillId { get; set; }
    public Skill? Skill { get; set; }
    public decimal Hours { get; set; }
    public string? Notes { get; set; }
    public DateTime RecordedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Validates progress business rules.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when one or more rules are invalid.</exception>
    public void Validate()
    {
        if (Hours <= 0 || Hours > 24)
            throw new ArgumentException("Hours must be greater than 0 and less than or equal to 24", nameof(Hours));

        if (RecordedAt > DateTime.UtcNow)
            throw new ArgumentException("RecordedAt cannot be in the future", nameof(RecordedAt));
    }
}
