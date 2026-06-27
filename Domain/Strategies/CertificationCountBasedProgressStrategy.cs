using Domain.Entities;

namespace Domain.Strategies;

/// <summary>
/// Concrete Strategy: calculates progress based on how many certifications
/// have been completed for a skill, rather than hours spent.
/// 
/// Useful for skills where the milestone that matters is "did you pass the exam?"
/// rather than "how many hours did you study?" — e.g., a skill like
/// "AWS Certifications" where completing 1 of 3 planned certs = 33%,
/// regardless of how long each one took.
/// </summary>
public class CertificationCountBasedProgressStrategy : IProgressCalculationStrategy
{
    private readonly int _targetCertificationCount;

    public CertificationCountBasedProgressStrategy(int targetCertificationCount)
    {
        if (targetCertificationCount <= 0)
            throw new ArgumentException("Target certification count must be greater than 0", nameof(targetCertificationCount));

        _targetCertificationCount = targetCertificationCount;
    }

    public int CalculateProgress(Skill skill, IEnumerable<Progress> progressEntries)
    {
        var completedCertifications = skill.Certifications.Count;
        var percentage = (completedCertifications * 100) / _targetCertificationCount;
        return Math.Min(100, percentage);
    }
}