using Application.Ports.Output;
using Domain.Entities;

namespace Infrastructure.Adapters.Output;

/// <summary>
/// In-memory implementation of ICertificationRepository.
/// 
/// This adapter exists exclusively for xUnit tests. It allows the domain
/// logic and use cases to be tested in complete isolation from AWS RDS,
/// which is the whole point of Hexagonal Architecture: the CertificationUseCase
/// does not know — and does not care — whether it's talking to this class
/// or to PostgreSQLCertificationRepository.
/// </summary>
public class InMemoryCertificationRepository : ICertificationRepository
{
    private readonly List<Certification> _certifications = new();
    private int _nextId = 1;

    public Task<IEnumerable<Certification>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Certification>>(_certifications);
    }

    public Task<Certification?> GetByIdAsync(int id)
    {
        var certification = _certifications.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(certification);
    }

    public Task<IEnumerable<Certification>> GetBySkillIdAsync(int skillId)
    {
        var result = _certifications.Where(c => c.Skills.Any(s => s.Id == skillId));
        return Task.FromResult(result);
    }

    public Task<Certification> AddAsync(Certification certification)
    {
        certification.Id = _nextId++;
        _certifications.Add(certification);
        return Task.FromResult(certification);
    }

    public Task<Certification?> UpdateAsync(int id, Certification certification)
    {
        var existing = _certifications.FirstOrDefault(c => c.Id == id);
        if (existing == null) return Task.FromResult<Certification?>(null);

        existing.Title = certification.Title;
        existing.CredentialUrl = certification.CredentialUrl;
        existing.UpdatedAt = certification.UpdatedAt;

        return Task.FromResult<Certification?>(existing);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var existing = _certifications.FirstOrDefault(c => c.Id == id);
        if (existing == null) return Task.FromResult(false);

        _certifications.Remove(existing);
        return Task.FromResult(true);
    }
}
