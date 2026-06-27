using Application.DTOs;
using Application.Ports.Input;
using Application.Ports.Output;
using Domain.Entities;

namespace Application.UseCases;

/// <summary>
/// Implements progress-tracking use cases.
/// 
/// RegisterProgressAsync follows exactly the flow documented in ADR-02's
/// Process View: validate hours -> fetch certification -> validate it exists
/// -> persist the progress entry -> return confirmation.
/// </summary>
public class ProgressUseCase : IProgressUseCase
{
    private readonly IProgressRepository _progressRepository;
    private readonly ICertificationRepository _certificationRepository;
    private readonly ICourseRepository _courseRepository;

    public ProgressUseCase(
        IProgressRepository progressRepository,
        ICertificationRepository certificationRepository,
        ICourseRepository courseRepository)
    {
        _progressRepository = progressRepository;
        _certificationRepository = certificationRepository;
        _courseRepository = courseRepository;
    }

    public async Task<ProgressDto> RegisterProgressAsync(CreateProgressRequest request)
    {
        if (request.CertificationId.HasValue)
        {
            var certification = await _certificationRepository.GetByIdAsync(request.CertificationId.Value);
            if (certification == null)
                throw new ArgumentException($"Certification with ID {request.CertificationId} not found", nameof(request.CertificationId));
        }

        if (request.CourseId.HasValue)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId.Value);
            if (course == null)
                throw new ArgumentException($"Course with ID {request.CourseId} not found", nameof(request.CourseId));
        }

        if (!request.CertificationId.HasValue && !request.CourseId.HasValue && !request.SkillId.HasValue)
            throw new ArgumentException("Progress must be linked to at least a Certification, Course, or Skill.");

        var progress = new Progress
        {
            CertificationId = request.CertificationId,
            CourseId = request.CourseId,
            SkillId = request.SkillId,
            Hours = request.Hours,
            Notes = request.Notes,
            RecordedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Step 2: domain-level validation (hours > 0, hours <= 24, etc.)
        progress.Validate();

        var created = await _progressRepository.AddAsync(progress);
        return MapToDto(created);
    }

    public async Task<ProgressSummaryDto> GetProgressSummaryAsync()
    {
        var certifications = await _certificationRepository.GetAllAsync();
        var allProgress = await _progressRepository.GetAllAsync();

        var certificationProgressItems = certifications.Select(cert => new CertificationProgressItem
        {
            CertificationId = cert.Id,
            Title = cert.Title,
            HoursSpent = allProgress.Where(p => p.CertificationId == cert.Id).Sum(p => p.Hours)
        }).ToList();

        return new ProgressSummaryDto
        {
            TotalCertifications = certifications.Count(),
            CompletedCertifications = certifications.Count(c => c.CompletedDate <= DateTime.UtcNow),
            InProgressCertifications = certifications.Count(c => c.CompletedDate > DateTime.UtcNow),
            TotalHoursSpent = allProgress.Sum(p => p.Hours),
            CertificationProgress = certificationProgressItems
        };
    }

    public async Task<IEnumerable<ProgressDto>> GetProgressByCertificationAsync(int certificationId)
    {
        var progressEntries = await _progressRepository.GetByCertificationIdAsync(certificationId);
        return progressEntries.Select(MapToDto);
    }

    public async Task<IEnumerable<ProgressDto>> GetProgressByCourseAsync(int courseId)
    {
        var progressEntries = await _progressRepository.GetByCourseIdAsync(courseId);
        return progressEntries.Select(MapToDto);
    }

    private static ProgressDto MapToDto(Progress progress)
    {
        return new ProgressDto
        {
            Id = progress.Id,
            CertificationId = progress.CertificationId,
            CourseId = progress.CourseId,
            SkillId = progress.SkillId,
            Hours = progress.Hours,
            Notes = progress.Notes,
            RecordedAt = progress.RecordedAt
        };
    }
}
