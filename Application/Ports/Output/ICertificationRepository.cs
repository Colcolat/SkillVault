using Domain.Entities;

namespace Application.Ports.Output;

/// <summary>
/// Output port defining persistence operations for Certification entities.
/// 
/// Implemented by adapters (PostgreSQL, InMemory) in the Infrastructure layer.
/// The Application layer depends on this interface, never on a concrete implementation.
/// </summary>
public interface ICertificationRepository
{
    /// <summary>
    /// Retrieves all certifications.
    /// </summary>
    Task<IEnumerable<Certification>> GetAllAsync();

    /// <summary>
    /// Retrieves a certification by its ID.
    /// </summary>
    /// <returns>The certification, or null if not found.</returns>
    Task<Certification?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves all certifications associated with a specific skill.
    /// </summary>
    Task<IEnumerable<Certification>> GetBySkillIdAsync(int skillId);

    /// <summary>
    /// Adds a new certification and returns it with its assigned ID.
    /// </summary>
    Task<Certification> AddAsync(Certification certification);

    /// <summary>
    /// Updates an existing certification.
    /// </summary>
    /// <returns>The updated certification, or null if not found.</returns>
    Task<Certification?> UpdateAsync(int id, Certification certification);

    /// <summary>
    /// Deletes a certification by ID.
    /// </summary>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(int id);
}
