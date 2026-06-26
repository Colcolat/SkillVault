using Domain.Entities;

namespace Application.Ports.Output;

/// <summary>
/// Output port defining persistence operations for Skill entities.
/// Implemented by adapters (PostgreSQL, InMemory) in the Infrastructure layer.
/// </summary>
public interface ISkillRepository
{
    Task<IEnumerable<Skill>> GetAllAsync();
    Task<Skill?> GetByIdAsync(int id);
    Task<Skill> AddAsync(Skill skill);
    Task<Skill?> UpdateAsync(int id, Skill skill);
    Task<bool> DeleteAsync(int id);
}