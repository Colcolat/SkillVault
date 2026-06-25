using Application.DTOs;

namespace Application.Ports.Input;

/// <summary>
/// Input port defining the use cases available for Certification management.
/// 
/// Implemented by the Application layer (CertificationUseCase) and consumed
/// by Infrastructure adapters (REST Controllers, future CLI tools, etc).
/// </summary>
public interface ICertificationUseCase
{
    /// <summary>
    /// Retrieves all certifications.
    /// </summary>
    Task<IEnumerable<CertificationDto>> GetAllCertificationsAsync();

    /// <summary>
    /// Retrieves a specific certification by ID.
    /// </summary>
    /// <returns>The certification DTO, or null if not found.</returns>
    Task<CertificationDto?> GetCertificationByIdAsync(int id);

    /// <summary>
    /// Retrieves certifications filtered by associated skill.
    /// </summary>
    Task<IEnumerable<CertificationDto>> GetCertificationsBySkillAsync(int skillId);

    /// <summary>
    /// Registers a new certification.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    Task<CertificationDto> RegisterCertificationAsync(CreateCertificationRequest request);

    /// <summary>
    /// Updates an existing certification.
    /// </summary>
    /// <returns>The updated certification, or null if not found.</returns>
    Task<CertificationDto?> UpdateCertificationAsync(int id, UpdateCertificationRequest request);

    /// <summary>
    /// Deletes a certification by ID.
    /// </summary>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteCertificationAsync(int id);
}