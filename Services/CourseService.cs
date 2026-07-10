using Microsoft.EntityFrameworkCore;
using Tms.Api.Data;
using Tms.Api.Dtos;
using Tms.Api.Entities;

namespace Tms.Api.Services;

public class CourseService : ICourseService
{
    private readonly TmsDbContext _db;
    private readonly ILogger<CourseService> _logger;

    public CourseService(TmsDbContext db, ILogger<CourseService> logger)
    {
        _db = db;
        _logger = logger;
    }
    public Task<bool> CodeExistsAsync(string Code, CancellationToken ct)=> _db.Courses.AsNoTracking().AnyAsync(c => c.Code == Code, ct);
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

        return new CourseResponseDto(
            course.Id,
            course.Code,
            course.Title,
            course.MaxCapacity,
            course.Enrollments.Count
        )
        ;
    }
    public async Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseResponseDto(
                c.Id,
                c.Code,
                c.Title,
                c.MaxCapacity,
                c.Enrollments.Count
            ))
            .FirstOrDefaultAsync(ct);
    }
public async Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(
PagedRequest request, CancellationToken ct)
{
   
    IQueryable<Course> query = _db.Courses.AsNoTracking();
    if(!string.IsNullOrWhiteSpace(request.Search))
    {
        query = query.Where(c => 
            EF.Functions.ILike(c.Title, $"%{request.Search}%") ||
            EF.Functions.ILike(c.Code, $"%{request.Search}%"));
    }

    var totalCount = await query.CountAsync(ct);

  IQueryable<Course> sortedQuery = request.OrderBy switch
    {
        "Code" => request.Descending
            ? query.OrderByDescending(c => c.Code)
            : query.OrderBy(c => c.Code),

        "MaxCapacity" => request.Descending
            ? query.OrderByDescending(c => c.MaxCapacity)
            : query.OrderBy(c => c.MaxCapacity),

        _ => request.Descending
            ? query.OrderByDescending(c => c.Title)
            : query.OrderBy(c => c.Title),
    };

    var items = await sortedQuery
        .Skip((request.Page- 1) * request.PageSize)
        .Take(request.PageSize)
        .Select(c => new CourseResponseDto(
            c.Id,
            c.Code,
            c.Title,
            c.MaxCapacity,
            c.Enrollments.Count
            ))
        .ToListAsync(ct);
  
        return new PagedResponse<CourseResponseDto> {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            }; 
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
    public async Task<IEnumerable<CourseResponseDto>> GetCourseEnrollments(int courseId, CancellationToken ct)
    {
        var course = await _db.Courses
            .AsNoTracking()
            .Where(c => c.Id == courseId)
            .Select(c => new CourseResponseDto(
                c.Id,
                c.Code,
                c.Title,
                c.MaxCapacity,
                c.Enrollments.Count
            ))
            .FirstOrDefaultAsync(ct);

        if (course is null)
        {
            _logger.LogWarning(
                "GetCourseEnrollments failed. Course {CourseId} not found",
                courseId);

            return Enumerable.Empty<CourseResponseDto>();
        }

        return new List<CourseResponseDto> { course };
    }
}

