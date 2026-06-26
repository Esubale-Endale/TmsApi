using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;


public interface IStudentService
{
    Task<Student> CreateAsync( string registrationNumber, string name, decimal gpa, bool isActive);
    Task<Student?> GetByIdAsync(int id);
    Task<IReadOnlyList<Student>> GetAllAsync(int page = 1, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id);
}


public class StudentService : IStudentService
{
    private readonly TmsDbContext _db;
    private readonly ILogger<StudentService> _logger;

    public StudentService( TmsDbContext db, ILogger<StudentService> logger)
    {
        _db = db;
        _logger = logger;
    }
    public async Task<IReadOnlyList<Student>> GetAllAsync( int page = 1, CancellationToken cancellationToken = default)
    {
        const int pageSize = 20;

        var students = await _db.Students
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return students;
    }
    public async Task<Student> CreateAsync( string registrationNumber, string name, decimal gpa, bool isActived)
    {
        var student = new Student
        {
            RegistrationNumber = registrationNumber,
            Name = name,
            GPA = gpa,
            IsActived = isActived
        };

        _db.Students.Add(student);

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Created student {RegistrationNumber} with id {StudentId}",
            registrationNumber,
            student.Id);

        return student;
    }
    public async Task<Student?> GetByIdAsync(int id)
    {
        var student = await _db.Students
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student is null)
        {
            _logger.LogWarning(
                "Student {StudentId} not found",
                id);
        }

        return student;
    }
    public async Task<IReadOnlyList<Student>> GetPagedAsync( int page, CancellationToken cancellationToken = default)
    {
        const int pageSize = 20;

        return await _db.Students
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student is null)
        {
            _logger.LogWarning(
                "Delete failed. Student {StudentId} not found",
                id);

            return false;
        }

        _db.Students.Remove(student);

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Deleted student {StudentId}",
            id);

        return true;
    }
    // public async Task UpdateStudentAsync(Student student, CancellationToken ct)
    // {
    //     _db.Entry(student)
    //         .Property("LastUpdated")
    //         .CurrentValue = DateTime.UtcNow;

    //     await _db.SaveChangesAsync(ct);
    // }
}