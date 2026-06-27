namespace Domain.Entities;

/// <summary>
/// Represents a completed certification in the user's learning journey.
/// </summary>
public class Certification
{
    public int Id { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime CompletedDate { get; set; }
    public string? CredentialUrl { get; set; }
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public ICollection<Progress> ProgressEntries { get; set; } = new List<Progress>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Validates certification business rules.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when one or more rules are invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Provider))
            throw new ArgumentException("Provider is required", nameof(Provider));

        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Title is required", nameof(Title));

        if (CompletedDate > DateTime.UtcNow)
            throw new ArgumentException("CompletedDate cannot be in the future", nameof(CompletedDate));
    }
}
