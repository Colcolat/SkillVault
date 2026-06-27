using Application.DTOs;

namespace Application.Ports.Input;

/// <summary>
/// Input port defining the use cases available for Progress tracking.
/// </summary>
public interface IProgressUseCase
{
    /// <summary>
    /// Registers a new study session / progress entry for a certification.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails or the certification doesn't exist.</exception>
    Task<ProgressDto> RegisterProgressAsync(CreateProgressRequest request);

    /// <summary>
    /// Retrieves the overall progress summary across all certifications.
    /// </summary>
    Task<ProgressSummaryDto> GetProgressSummaryAsync();

    /// <summary>
    /// Retrieves progress entries for a specific certification.
    /// </summary>
    Task<IEnumerable<ProgressDto>> GetProgressByCertificationAsync(int certificationId);
    Task<IEnumerable<ProgressDto>> GetProgressByCourseAsync(int courseId);
}