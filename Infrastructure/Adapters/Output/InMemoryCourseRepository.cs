using Application.Ports.Output;
using Domain.Entities;

namespace Infrastructure.Adapters.Output;

public class InMemoryCourseRepository : ICourseRepository
{
    private readonly List<Course> _courses = new();
    private int _nextId = 1;

    public Task<Course?> GetByIdAsync(int id)
    {
        var course = _courses.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(course);
    }

    public Task<IEnumerable<Course>> GetAllAsync()
    {
        return Task.FromResult(_courses.AsEnumerable());
    }

    public Task<Course> AddAsync(Course course)
    {
        course.Id = _nextId++;
        _courses.Add(course);
        return Task.FromResult(course);
    }

    public Task UpdateAsync(Course course)
    {
        var existing = _courses.FirstOrDefault(c => c.Id == course.Id);
        if (existing != null)
        {
            existing.Title = course.Title;
            existing.Provider = course.Provider;
            existing.Status = course.Status;
            existing.Url = course.Url;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var course = _courses.FirstOrDefault(c => c.Id == id);
        if (course != null)
        {
            _courses.Remove(course);
        }
        return Task.CompletedTask;
    }
}
