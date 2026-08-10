
using Microsoft.EntityFrameworkCore;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Infrastructure.Repositories;

public class EnrollmentRepository(TmsDbContext db) : IEnrollmentRepository
{
    public Task<List<Enrollment>> GetAllAsync(CancellationToken ct) => db.Enrollments.AsNoTracking().ToListAsync(ct);

    public Task<Enrollment?> GetByIdAsync(int id, CancellationToken ct) => db.Enrollments.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<bool> ExistsAsync(int studentId, int courseId, CancellationToken ct) => db.Enrollments.AsNoTracking().AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct);

    public async Task AddAsync(Enrollment enrollment, CancellationToken ct)
    {
        await db.Enrollments.AddAsync(enrollment, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Enrollment enrollment, CancellationToken ct)
    {
        db.Enrollments.Update(enrollment);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Enrollment enrollment, CancellationToken ct)
    {
        db.Enrollments.Remove(enrollment);
        await db.SaveChangesAsync(ct);
    }
}