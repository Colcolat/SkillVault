using Application.DTOs;

namespace Application.Ports.Input;

public interface ICourseUseCase
{
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
    Task<CourseDto> AddCourseAsync(CreateCourseRequest request);
    Task UpdateCourseStatusAsync(int id, string status);
    Task DeleteCourseAsync(int id);
}
