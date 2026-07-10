using Microsoft.EntityFrameworkCore;
using Tms.Api.Data;
using Tms.Api.Dtos;
using Tms.Api.Entities;
namespace Tms.Api.Services;

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
    public async Task<StudentResponseDto> CreateAsync(CreateStudentRequest request, CancellationToken ct)
    {
        var student = new Student
        {
            RegistrationNumber = request.RegistrationNumber,
            Name = request.Name,
            GPA = request.GPA,
            IsActived = request.IsActive
        };

        _db.Students.Add(student);

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Created student {RegistrationNumber} with id {StudentId}", student.RegistrationNumber, student.Id);

        return new StudentResponseDto(
            student.Id,
            student.RegistrationNumber,
            student.Name, 
            student.GPA,
            student.IsActived,
            student.Enrollments.Count
        )
        ;
    }
    public async Task<StudentResponseDto?> GetByIdAsync(int id,CancellationToken ct)
    {
        return await _db.Students
           .AsNoTracking()
           .Where(s=> s.Id == id)
           .Select(s=> new StudentResponseDto(
                s.Id,
                s.RegistrationNumber,
                s.Name,
                s.GPA,
                s.IsActived,
                s.Enrollments.Count
           )).FirstOrDefaultAsync(ct); 
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
         student.IsDeleted = true;

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