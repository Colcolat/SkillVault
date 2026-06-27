using Application.Ports.Output;
using Domain.Entities;

namespace Infrastructure.Adapters.Output;

/// <summary>
/// In-memory implementation of IProgressRepository, used exclusively for
/// xUnit tests to isolate ProgressUseCase from AWS RDS.
/// </summary>
public class InMemoryProgressRepository : IProgressRepository
{
    private readonly List<Progress> _progressEntries = new();
    private int _nextId = 1;

    public Task<IEnumerable<Progress>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Progress>>(_progressEntries);
    }

    public Task<Progress?> GetByIdAsync(int id)
    {
        var progress = _progressEntries.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(progress);
    }

    public Task<IEnumerable<Progress>> GetByCertificationIdAsync(int certificationId)
    {
        var result = _progressEntries.Where(p => p.CertificationId == certificationId);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Progress>> GetByCourseIdAsync(int courseId)
    {
        var result = _progressEntries.Where(p => p.CourseId == courseId);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Progress>> GetBySkillIdAsync(int skillId)
    {
        var result = _progressEntries.Where(p => p.SkillId == skillId);
        return Task.FromResult(result);
    }

    public Task<Progress> AddAsync(Progress progress)
    {
        progress.Id = _nextId++;
        _progressEntries.Add(progress);
        return Task.FromResult(progress);
    }
}