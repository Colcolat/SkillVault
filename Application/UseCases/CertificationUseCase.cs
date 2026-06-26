using Application.DTOs;
using Application.Ports.Input;
using Application.Ports.Output;
using Domain.Entities;

namespace Application.UseCases;

/// <summary>
/// Implements certification-related use cases.
/// 
/// This class is the heart of the Application layer: it orchestrates calls
/// to the Domain (validation, business rules) and the Output Port (persistence),
/// without knowing whether persistence is PostgreSQL or InMemory.
/// </summary>
public class CertificationUseCase : ICertificationUseCase
{
    private readonly ICertificationRepository _repository;

    public CertificationUseCase(ICertificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CertificationDto>> GetAllCertificationsAsync()
    {
        var certifications = await _repository.GetAllAsync();
        return certifications.Select(MapToDto);
    }

    public async Task<CertificationDto?> GetCertificationByIdAsync(int id)
    {
        var certification = await _repository.GetByIdAsync(id);
        return certification == null ? null : MapToDto(certification);
    }

    public async Task<IEnumerable<CertificationDto>> GetCertificationsBySkillAsync(int skillId)
    {
        var certifications = await _repository.GetBySkillIdAsync(skillId);
        return certifications.Select(MapToDto);
    }

    public async Task<CertificationDto> RegisterCertificationAsync(CreateCertificationRequest request)
    {
        var certification = new Certification
        {
            Provider = request.Provider,
            Title = request.Title,
            CompletedDate = NormalizeToUtc(request.CompletedDate),
            CredentialUrl = request.CredentialUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Domain validation happens here - before touching persistence
        certification.Validate();

        var created = await _repository.AddAsync(certification);
        return MapToDto(created);
    }

    public async Task<CertificationDto?> UpdateCertificationAsync(int id, UpdateCertificationRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        if (!string.IsNullOrWhiteSpace(request.Title))
            existing.Title = request.Title;

        if (request.CredentialUrl != null)
            existing.CredentialUrl = request.CredentialUrl;

        existing.UpdatedAt = DateTime.UtcNow;
        existing.Validate();

        var updated = await _repository.UpdateAsync(id, existing);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task<bool> DeleteCertificationAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Maps a Domain entity to a DTO for API responses.
    /// Keeps the Domain entity from leaking directly into the API layer.
    /// </summary>
    private static CertificationDto MapToDto(Certification certification)
    {
        return new CertificationDto
        {
            Id = certification.Id,
            Provider = certification.Provider,
            Title = certification.Title,
            CompletedDate = certification.CompletedDate,
            CredentialUrl = certification.CredentialUrl,
            SkillIds = certification.Skills.Select(s => s.Id),
            CreatedAt = certification.CreatedAt,
            UpdatedAt = certification.UpdatedAt
        };
    }

    private static DateTime NormalizeToUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
