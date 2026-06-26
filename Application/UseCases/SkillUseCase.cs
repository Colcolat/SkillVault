using Application.DTOs;
using Application.Ports.Input;
using Application.Ports.Output;
using Domain.Entities;

namespace Application.UseCases;

/// <summary>
/// Implements skill-related use cases. Orchestrates domain validation
/// and persistence through the ISkillRepository output port.
/// </summary>
public class SkillUseCase : ISkillUseCase
{
    private readonly ISkillRepository _repository;

    public SkillUseCase(ISkillRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SkillDto>> GetAllSkillsAsync()
    {
        var skills = await _repository.GetAllAsync();
        return skills.Select(MapToDto);
    }

    public async Task<SkillDto?> GetSkillByIdAsync(int id)
    {
        var skill = await _repository.GetByIdAsync(id);
        return skill == null ? null : MapToDto(skill);
    }

    public async Task<SkillDto> CreateSkillAsync(CreateSkillRequest request)
    {
        var skill = new Skill
        {
            Name = request.Name,
            Description = request.Description,
            Level = request.Level,
            TargetHours = request.TargetHours,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Domain validation before touching persistence
        skill.Validate();

        var created = await _repository.AddAsync(skill);
        return MapToDto(created);
    }

    public async Task<SkillDto?> UpdateSkillAsync(int id, UpdateSkillRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        if (!string.IsNullOrWhiteSpace(request.Name))
            existing.Name = request.Name;

        if (request.Description != null)
            existing.Description = request.Description;

        if (!string.IsNullOrWhiteSpace(request.Level))
            existing.Level = request.Level;

        if (request.TargetHours.HasValue)
            existing.TargetHours = request.TargetHours.Value;

        existing.UpdatedAt = DateTime.UtcNow;
        existing.Validate();

        var updated = await _repository.UpdateAsync(id, existing);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task<bool> DeleteSkillAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static SkillDto MapToDto(Skill skill)
    {
        return new SkillDto
        {
            Id = skill.Id,
            Name = skill.Name,
            Description = skill.Description,
            Level = skill.Level,
            TargetHours = skill.TargetHours,
            CertificationCount = skill.Certifications.Count,
            CreatedAt = skill.CreatedAt,
            UpdatedAt = skill.UpdatedAt
        };
    }
}