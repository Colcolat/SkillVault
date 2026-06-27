using Microsoft.EntityFrameworkCore;
using Application.Ports.Output;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Adapters.Output;

/// <summary>
/// PostgreSQL implementation of ICertificationRepository.
/// 
/// This adapter translates domain operations into EF Core / SQL queries
/// against AWS RDS PostgreSQL. Used in production.
/// </summary>
public class PostgreSQLCertificationRepository : ICertificationRepository
{
    private readonly SkillVaultDbContext _context;

    public PostgreSQLCertificationRepository(SkillVaultDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Certification>> GetAllAsync()
    {
        return await _context.Certifications
            .Include(c => c.Skills)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Certification?> GetByIdAsync(int id)
    {
        return await _context.Certifications
            .Include(c => c.Skills)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Certification>> GetBySkillIdAsync(int skillId)
    {
        return await _context.Certifications
            .Include(c => c.Skills)
            .Where(c => c.Skills.Any(s => s.Id == skillId))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Certification> AddAsync(Certification certification)
    {
        _context.Certifications.Add(certification);
        await _context.SaveChangesAsync();
        return certification;
    }

    public async Task<Certification?> UpdateAsync(int id, Certification certification)
    {
        var existing = await _context.Certifications.FirstOrDefaultAsync(c => c.Id == id);
        if (existing == null) return null;

        existing.Title = certification.Title;
        existing.CredentialUrl = certification.CredentialUrl;
        existing.UpdatedAt = certification.UpdatedAt;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Certifications.FirstOrDefaultAsync(c => c.Id == id);
        if (existing == null) return false;

        _context.Certifications.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
