using Microsoft.EntityFrameworkCore;
using Application.Ports.Output;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Adapters.Output;

/// <summary>
/// PostgreSQL implementation of IProgressRepository.
/// Translates domain operations into EF Core / SQL queries against AWS RDS.
/// </summary>
public class PostgreSQLProgressRepository : IProgressRepository
{
    private readonly SkillVaultDbContext _context;

    public PostgreSQLProgressRepository(SkillVaultDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Progress>> GetAllAsync()
    {
        return await _context.ProgressEntries
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Progress?> GetByIdAsync(int id)
    {
        return await _context.ProgressEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Progress>> GetByCertificationIdAsync(int certificationId)
    {
        return await _context.ProgressEntries
            .Where(p => p.CertificationId == certificationId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Progress>> GetByCourseIdAsync(int courseId)
    {
        return await _context.ProgressEntries
            .Where(p => p.CourseId == courseId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Progress>> GetBySkillIdAsync(int skillId)
    {
        return await _context.ProgressEntries
            .Where(p => p.SkillId == skillId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Progress> AddAsync(Progress progress)
    {
        _context.ProgressEntries.Add(progress);
        await _context.SaveChangesAsync();
        return progress;
    }
}