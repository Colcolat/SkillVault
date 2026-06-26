using Domain.Entities;

namespace Domain.Strategies;

/// <summary>
/// Concrete Strategy: calculates progress as a simple ratio of hours
/// accumulated against the skill's TargetHours.
/// 
/// Used for most skills (e.g., "Java", "Cloud Computing") where progress
/// is naturally measured by time invested.
/// </summary>
public class HoursBasedProgressStrategy : IProgressCalculationStrategy
{
    public int CalculateProgress(Skill skill, IEnumerable<Progress> progressEntries)
    {
        if (skill.TargetHours <= 0) return 0;

        var totalHours = progressEntries
            .Where(p => p.SkillId == skill.Id)
            .Sum(p => p.Hours);

        var percentage = (totalHours * 100) / skill.TargetHours;
        return (int)Math.Min(100, percentage);
    }
}