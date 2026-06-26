using Domain.Entities;

namespace Domain.Strategies;

/// <summary>
/// GOF Design Pattern: Strategy (Behavioral)
/// 
/// Defines a family of interchangeable algorithms for calculating how
/// "complete" a Skill is, without the Skill entity or any UseCase needing
/// to know which specific algorithm is being used.
/// </summary>
public interface IProgressCalculationStrategy
{
    /// <summary>
    /// Calculates the completion percentage (0-100) for a skill,
    /// given its recorded progress entries.
    /// </summary>
    int CalculateProgress(Skill skill, IEnumerable<Progress> progressEntries);
}