using Application.Ports.Output;
using Domain.Entities;

namespace Infrastructure.Adapters.Output;

/// <summary>
/// In-memory implementation of ISkillRepository, used exclusively for
/// xUnit tests to isolate the SkillUseCase from AWS RDS.
/// </summary>
public class InMemorySkillRepository : ISkillRepository
{
    private readonly List<Skill> _skills = new();
    private int _nextId = 1;

    public Task<IEnumerable<Skill>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Skill>>(_skills);
    }

    public Task<Skill?> GetByIdAsync(int id)
    {
        var skill = _skills.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(skill);
    }

    public Task<Skill> AddAsync(Skill skill)
    {
        skill.Id = _nextId++;
        _skills.Add(skill);
        return Task.FromResult(skill);
    }

    public Task<Skill?> UpdateAsync(int id, Skill skill)
    {
        var existing = _skills.FirstOrDefault(s => s.Id == id);
        if (existing == null) return Task.FromResult<Skill?>(null);

        existing.Name = skill.Name;
        existing.Description = skill.Description;
        existing.Level = skill.Level;
        existing.TargetHours = skill.TargetHours;
        existing.UpdatedAt = skill.UpdatedAt;

        return Task.FromResult<Skill?>(existing);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var existing = _skills.FirstOrDefault(s => s.Id == id);
        if (existing == null) return Task.FromResult(false);

        _skills.Remove(existing);
        return Task.FromResult(true);
    }
}
