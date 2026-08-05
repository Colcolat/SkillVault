using Application.Ports.Output;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters.Output;

public class PostgreSQLUserProfileRepository : IUserProfileRepository
{
    private readonly SkillVaultDbContext _dbContext;

    public PostgreSQLUserProfileRepository(SkillVaultDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfile?> GetByEmailAsync(string email)
    {
        return await _dbContext.UserProfiles
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(UserProfile userProfile)
    {
        await _dbContext.UserProfiles.AddAsync(userProfile);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserProfile userProfile)
    {
        _dbContext.UserProfiles.Update(userProfile);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserProfile>> GetInactiveUsersAsync(DateTime threshold)
    {
        return await _dbContext.UserProfiles
            .Where(u => u.RemindersEnabled && u.LastActiveDate < threshold)
            .ToListAsync();
    }
}
