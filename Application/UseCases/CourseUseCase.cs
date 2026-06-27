using Application.DTOs;
using Application.Ports.Input;
using Application.Ports.Output;
using Domain.Entities;

namespace Application.UseCases;

public class CourseUseCase : ICourseUseCase
{
    private readonly ICourseRepository _courseRepository;

    public CourseUseCase(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
    {
        var courses = await _courseRepository.GetAllAsync();
        return courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Provider = c.Provider,
            Status = c.Status,
            Url = c.Url,
            TotalHours = c.ProgressEntries.Sum(p => p.Hours),
            CreatedAt = c.CreatedAt
        });
    }

    public async Task<CourseDto> AddCourseAsync(CreateCourseRequest request)
    {
        var course = new Course
        {
            Title = request.Title,
            Provider = request.Provider,
            Url = request.Url,
            Status = "InProgress"
        };

        course.Validate();
        var added = await _courseRepository.AddAsync(course);

        return new CourseDto
        {
            Id = added.Id,
            Title = added.Title,
            Provider = added.Provider,
            Status = added.Status,
            Url = added.Url,
            TotalHours = 0,
            CreatedAt = added.CreatedAt
        };
    }

    public async Task UpdateCourseStatusAsync(int id, string status)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
            throw new KeyNotFoundException($"Course with ID {id} not found.");

        course.Status = status;
        course.Validate();

        await _courseRepository.UpdateAsync(course);
    }

    public async Task DeleteCourseAsync(int id)
    {
        await _courseRepository.DeleteAsync(id);
    }
}
