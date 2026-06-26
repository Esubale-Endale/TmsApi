using Microsoft.EntityFrameworkCore;

using TmsApi.Data;
using TmsApi.Entities;
using TmsApi.Services;

public class CourseService : ICourseService
{
    private readonly TmsDbContext _db;
    private readonly ILogger<CourseService> _logger;

    public CourseService(TmsDbContext db, ILogger<CourseService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Course> CreateAsync(Course course, CancellationToken ct)
    {
        _db.Courses.Add(course);
        
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Created course {CourseId} ({Code})",
            course.Id,
            course.Code);

        return course;
    }
    public async Task<Course?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
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