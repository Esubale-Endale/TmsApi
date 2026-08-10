using Microsoft.EntityFrameworkCore;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Infrastructure.Repositories;

public class StudentRepository(TmsDbContext context) : IStudentRepository
{
    public async Task<List<Student>> GetAllAsync(CancellationToken ct)
    {
        return await context.Students
            .ToListAsync(ct);
    }
    public async Task<Student?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await context.Students
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task AddAsync(Student student, CancellationToken ct)
    {
        await context.Students.AddAsync(student, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Student student, CancellationToken ct)
    {
        context.Students.Update(student);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Student student, CancellationToken ct)
    {
        context.Students.Remove(student);
        await context.SaveChangesAsync(ct);
    }
}