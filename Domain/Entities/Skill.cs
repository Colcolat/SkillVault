namespace Domain.Entities;

/// <summary>
/// Represents a skill category used to group and track learning progress.
/// </summary>
public class Skill
{
    private static readonly string[] AllowedLevels = { "Beginner", "Intermediate", "Advanced", "Expert" };

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Level { get; set; } = "Beginner";
    public int TargetHours { get; set; }
    public ICollection<Certification> Certifications { get; set; } = new List<Certification>();
    public ICollection<Progress> ProgressEntries { get; set; } = new List<Progress>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Validates skill business rules.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when one or more rules are invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name is required", nameof(Name));

        if (string.IsNullOrWhiteSpace(Level) || !AllowedLevels.Contains(Level))
            throw new ArgumentException("Level must be one of: Beginner, Intermediate, Advanced, Expert", nameof(Level));

        if (TargetHours <= 0)
            throw new ArgumentException("TargetHours must be greater than 0", nameof(TargetHours));
    }

    /// <summary>
    /// Calculates progress percentage based on current hours and target hours.
    /// </summary>
    public int CalculateProgress(int currentHours)
    {
        if (TargetHours <= 0)
            return 0;

        var percentage = (currentHours * 100.0) / TargetHours;
        return (int)Math.Clamp(Math.Round(percentage), 0, 100);
    }
}
