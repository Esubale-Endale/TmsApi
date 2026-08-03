
using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface IEnrollmentRepository
{
    Task<List<Enrollment>> GetAllAsync(CancellationToken ct);
    Task<Enrollment?> GetByIdAsync(int id, CancellationToken ct);

    Task<bool> ExistsAsync(
        int studentId,
        int courseId,
        CancellationToken ct);

    Task AddAsync(Enrollment enrollment, CancellationToken ct);
    Task UpdateAsync(Enrollment enrollment, CancellationToken ct);
    Task DeleteAsync(Enrollment enrollment, CancellationToken ct);
}