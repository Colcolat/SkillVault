using Application.DTOs;

namespace Application.Ports.Input;

/// <summary>
/// Input port defining the use cases available for Skill management.
/// </summary>
public interface ISkillUseCase
{
    Task<IEnumerable<SkillDto>> GetAllSkillsAsync();
    Task<SkillDto?> GetSkillByIdAsync(int id);
    Task<SkillDto> CreateSkillAsync(CreateSkillRequest request);
    Task<SkillDto?> UpdateSkillAsync(int id, UpdateSkillRequest request);
    Task<bool> DeleteSkillAsync(int id);
}