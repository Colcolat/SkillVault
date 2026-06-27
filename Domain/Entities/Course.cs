namespace Domain.Entities;

/// <summary>
/// Represents a course currently in progress or planned by the user.
/// </summary>
public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Status { get; set; } = "InProgress"; // InProgress, Completed, Dropped
    public string? Url { get; set; }
    public ICollection<Progress> ProgressEntries { get; set; } = new List<Progress>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Validates course business rules.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when one or more rules are invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Title is required", nameof(Title));

        if (string.IsNullOrWhiteSpace(Provider))
            throw new ArgumentException("Provider is required", nameof(Provider));

        if (string.IsNullOrWhiteSpace(Status))
            throw new ArgumentException("Status is required", nameof(Status));
    }
}
