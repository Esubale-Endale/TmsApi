
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;

public interface ICourseService
{
    Task<Course> CreateAsync(
        string code,
        string title,
        int capacity);
    Task<Course?> GetByIdAsync(int id);
    Task<IReadOnlyList<Course>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
}

public class CourseService : ICourseService
{
    private readonly TmsDbContext _db;
    private readonly ILogger<CourseService> _logger;

    public CourseService(
        TmsDbContext db,
        ILogger<CourseService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Course> CreateAsync(
        string code,
        string title,
        int capacity)
    {
        var course = new Course
        {
            Code = code,
            Title = title,
            Capacity = capacity
        };

        _db.Courses.Add(course);

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Created course {CourseCode} with id {CourseId}",
            code,
            course.Id);

        return course;
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        var course = await _db.Courses
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course is null)
        {
            _logger.LogWarning(
                "Course {CourseId} not found",
                id);
        }

        return course;
    }

    public async Task<IReadOnlyList<Course>> GetAllAsync()
    {
        return await _db.Courses
            .OrderBy(c => c.Title)
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await _db.Courses
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course is null)
        {
            _logger.LogWarning(
                "Delete failed. Course {CourseId} not found",
                id);

            return false;
        }

        _db.Courses.Remove(course);

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Deleted course {CourseId}",
            id);

        return true;
    }
}