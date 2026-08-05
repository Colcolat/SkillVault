using Domain.Entities;

namespace Application.Ports.Output;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByEmailAsync(string email);
    Task AddAsync(UserProfile userProfile);
    Task UpdateAsync(UserProfile userProfile);
    Task<IEnumerable<UserProfile>> GetInactiveUsersAsync(DateTime threshold);
}
