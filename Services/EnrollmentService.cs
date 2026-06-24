using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;

public interface IEnrollmentService
{
    Task<Enrollment> EnrollAsync(int studentId, int courseId);
    Task<Enrollment?> GetByIdAsync(int id);
    Task<IReadOnlyList<Enrollment>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
}

public class EnrollmentService : IEnrollmentService
{
    private readonly TmsDbContext _db;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(TmsDbContext db, ILogger<EnrollmentService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Enrollment> EnrollAsync(int studentId, int courseId)
    {
        var existing = await _db.Enrollments
            .FirstOrDefaultAsync(e =>
                e.StudentId == studentId &&
                e.CourseId == courseId);

        if (existing is not null)
        {
            _logger.LogWarning(
                "Duplicate enrollment attempt. Student {StudentId} already enrolled in Course {CourseId}",
                studentId,
                courseId);

            return existing;
        }

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow
        };

        _db.Enrollments.Add(enrollment);

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Student {StudentId} enrolled in Course {CourseId}. Enrollment Id {EnrollmentId}",
            studentId,
            courseId,
            enrollment.Id);

        return enrollment;
    }

    public async Task<Enrollment?> GetByIdAsync(int id)
    {
        var enrollment = await _db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enrollment is null)
        {
            _logger.LogWarning(
                "Enrollment {EnrollmentId} not found",
                id);
        }

        return enrollment;
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
}

public class TmsDatabaseException : Exception
{
    public TmsDatabaseException(string message)
        : base(message)
    {
    }
}