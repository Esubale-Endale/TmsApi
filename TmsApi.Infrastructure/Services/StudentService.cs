using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Dtos;
using TmsApi.Entities;
namespace TmsApi.Services;

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
    public Task<bool> RegistrationNumberExistsAsync(string registrationNumber, CancellationToken ct)
    {
        return _db.Students.AsNoTracking().AnyAsync(s => s.RegistrationNumber == registrationNumber, ct);
    }
    public async Task<PagedResponse<StudentResponseDto>> GetStudentsAsync(PagedRequest reqest, CancellationToken ct = default)
    {
        IQueryable<Student> query = _db.Students.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(reqest.Search))
        {
            query = query.Where(s =>
                EF.Functions.ILike(s.Name, $"%{reqest.Search}%") ||
                EF.Functions.ILike(s.RegistrationNumber, $"%{reqest.Search}%"));
        }

        var totalCount = await query.CountAsync(ct);

        IQueryable<Student> sortedQuery = reqest.OrderBy switch
        {
            "RegistrationNumber" => reqest.Descending
                ? query.OrderByDescending(s => s.RegistrationNumber)
                : query.OrderBy(s => s.RegistrationNumber),
            "GPA" => reqest.Descending
                ? query.OrderByDescending(s => s.GPA)
                : query.OrderBy(s => s.GPA),
            _ => reqest.Descending
                ? query.OrderByDescending(s => s.Name)
                : query.OrderBy(s => s.Name),
        };

        var items = await sortedQuery
            .Skip((reqest.Page - 1) * reqest.PageSize)
            .Take(reqest.PageSize)
            .Select(s => new StudentResponseDto(
                s.Id,
                s.RegistrationNumber,
                s.Name,
                s.GPA,
                s.IsActived,
                s.Enrollments.Count
            ))
            .ToListAsync(ct);

        return new PagedResponse<StudentResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = reqest.Page,
            PageSize = reqest.PageSize
        };
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