using Tms.Api.Dtos;
using Tms.Api.Entities;

namespace Tms.Api.Services;
public interface IEnrollmentService
{
    Task<EnrollmentResponseDto?> GetByIdAsync(
        int courseId,
        int id,
        CancellationToken ct
        );

    Task<EnrollmentResponseDto> CreateAsync(
        int courseId,
        EnrollStudentRequest request,
        CancellationToken ct
        );

    Task<IReadOnlyList<EnrollmentResponseDto>> GetByCourseAsync(
        int courseId,
        CancellationToken ct
        );

    Task<IReadOnlyList<Enrollment>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
    public Task ArchiveOldEnrollmentsAsync(DateTime cutoff, CancellationToken ct);
}