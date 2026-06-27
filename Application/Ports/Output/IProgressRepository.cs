using Domain.Entities;

namespace Application.Ports.Output;

/// <summary>
/// Output port defining persistence operations for Progress entries.
/// </summary>
public interface IProgressRepository
{
    Task<IEnumerable<Progress>> GetAllAsync();
    Task<Progress?> GetByIdAsync(int id);
    Task<IEnumerable<Progress>> GetByCertificationIdAsync(int certificationId);
    Task<IEnumerable<Progress>> GetByCourseIdAsync(int courseId);
    Task<IEnumerable<Progress>> GetBySkillIdAsync(int skillId);
    Task<Progress> AddAsync(Progress progress);
}