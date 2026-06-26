using Domain.Entities;
using Domain.Strategies;

namespace SkillVault.Domain.Services;

/// <summary>
/// Domain service responsible for calculating skill progress.
/// 
/// This is the consumer side of the Strategy pattern: SkillProgressService
/// does not know or care HOW progress is calculated (hours-based,
/// certification-count-based, or any future strategy). It simply delegates
/// to whichever IProgressCalculationStrategy was injected.
/// </summary>
public class SkillProgressService
{
    private readonly IProgressCalculationStrategy _strategy;

    public SkillProgressService(IProgressCalculationStrategy strategy)
    {
        _strategy = strategy;
    }

    /// <summary>
    /// Calculates the current progress percentage for a skill using
    /// whichever strategy was configured for this service instance.
    /// </summary>
    public int GetProgressPercentage(Skill skill, IEnumerable<Progress> progressEntries)
    {
        return _strategy.CalculateProgress(skill, progressEntries);
    }

    /// <summary>
    /// Determines if a skill has reached completion (100%) under the
    /// current strategy.
    /// </summary>
    public bool IsSkillCompleted(Skill skill, IEnumerable<Progress> progressEntries)
    {
        return GetProgressPercentage(skill, progressEntries) >= 100;
    }
}