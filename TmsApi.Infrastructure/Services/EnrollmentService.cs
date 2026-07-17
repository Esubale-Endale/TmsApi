using Microsoft.EntityFrameworkCore;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Domain.Entities;
using TmsApi.Application.DTOs;
using TmsApi.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using TmsApi.Application.Interfaces;

namespace TmsApi.Infrastructure.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly TmsDbContext _db;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(TmsDbContext db, ILogger<EnrollmentService> logger)
    {
        _db = db;
        _logger = logger;
    }
public Task<EnrollmentResponseDto?> GetByIdAsync(int courseId, int id, CancellationToken ct) =>
    _db.Enrollments
        .AsNoTracking()
        .Where(e => e.Id == id && e.CourseId == courseId)
        .Select(e => new EnrollmentResponseDto(e.Id, e.CourseId, e.
        StudentId, e.EnrolledAt))
        .FirstOrDefaultAsync(ct);

public async Task<IReadOnlyList<EnrollmentResponseDto>> GetByCourseAsync( int courseId, CancellationToken ct)
{
    return await _db.Enrollments
        .AsNoTracking()
        .Where(e => e.CourseId == courseId)
        .Select(e => new EnrollmentResponseDto(
            e.Id,
            e.StudentId,
            e.CourseId,
            e.EnrolledAt))
        .ToListAsync(ct);
}
public async Task<EnrollmentResponseDto> CreateAsync(int courseId, EnrollStudentRequest request, CancellationToken ct)
{
    var enrollment = new Enrollment
    {
        CourseId = courseId,
        StudentId = request.StudentId,
        EnrolledAt = DateTime.UtcNow
    };

    _db.Enrollments.Add(enrollment);

    await _db.SaveChangesAsync(ct);

    _logger.LogInformation(
        "Student {StudentId} enrolled in course {CourseId}",
        request.StudentId,
        courseId);

    return (await GetByIdAsync(courseId, enrollment.Id, ct))!;
}
    public async Task<IReadOnlyList<Enrollment>> GetAllAsync()
    {
        return await _db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .ToListAsync();
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var enrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enrollment is null)
        {
            _logger.LogWarning(
                "Delete failed. Enrollment {EnrollmentId} not found",
                id);

            return false;
        }

        _db.Enrollments.Remove(enrollment);

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Deleted enrollment {EnrollmentId}",
            id);

        return true;
    }
    public async Task ArchiveOldEnrollmentsAsync(DateTime cutoff, CancellationToken ct)
    {
        await _db.Enrollments
            .Where(e => e.EnrolledAt < cutoff)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(e => e.IsArchived, true),
                ct);
    }
}
public class TmsDatabaseException : Exception
{
    public TmsDatabaseException(string message)
        : base(message)
    {
    }
}