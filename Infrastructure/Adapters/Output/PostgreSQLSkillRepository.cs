using Microsoft.EntityFrameworkCore;
using Application.Ports.Output;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Adapters.Output;

/// <summary>
/// PostgreSQL implementation of ISkillRepository.
/// Translates domain operations into EF Core / SQL queries against AWS RDS.
/// </summary>
public class PostgreSQLSkillRepository : ISkillRepository
{
    private readonly SkillVaultDbContext _context;

    public PostgreSQLSkillRepository(SkillVaultDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Skill>> GetAllAsync()
    {
        return await _context.Skills
            .Include(s => s.Certifications)
            .Include(s => s.ProgressEntries)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Skill?> GetByIdAsync(int id)
    {
        return await _context.Skills
            .Include(s => s.Certifications)
            .Include(s => s.ProgressEntries)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Skill> AddAsync(Skill skill)
    {
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();
        return skill;
    }

    public async Task<Skill?> UpdateAsync(int id, Skill skill)
    {
        var existing = await _context.Skills.FirstOrDefaultAsync(s => s.Id == id);
        if (existing == null) return null;

        existing.Name = skill.Name;
        existing.Description = skill.Description;
        existing.Level = skill.Level;
        existing.TargetHours = skill.TargetHours;
        existing.UpdatedAt = skill.UpdatedAt;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Skills.FirstOrDefaultAsync(s => s.Id == id);
        if (existing == null) return false;

        _context.Skills.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
