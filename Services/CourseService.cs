using Microsoft.EntityFrameworkCore;

using TmsApi.Data;
using TmsApi.Dtos;
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

    public async Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct)
    {
        var course = new Course
        {
            Code = request.Code,
            Title = request.Title,
            MaxCapacity = request.MaxCapacity
        };

        _db.Courses.Add(course);

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Created course {CourseId} ({Code})",
            course.Id,
            course.Code);

        return new CourseResponseDto
        {
            Id = course.Id,
            Code = course.Code,
            Title = course.Title,
            MaxCapacity = course.MaxCapacity,
            EnrollmentCount = 0
        };
    }
    public async Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Code = c.Code,
                Title = c.Title,
                MaxCapacity = c.MaxCapacity,
                EnrollmentCount = c.Enrollments.Count
            })
            .FirstOrDefaultAsync(ct);
    }
    public async Task<IEnumerable<CourseResponseDto>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Courses
            .AsNoTracking()
            .OrderBy(c => c.Code)
            .Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Code = c.Code,
                Title = c.Title,
                MaxCapacity = c.MaxCapacity,
                EnrollmentCount = c.Enrollments.Count
            })
            .ToListAsync(ct);
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