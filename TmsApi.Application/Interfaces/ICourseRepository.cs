using TmsApi.Application.DTOs;
using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync(CancellationToken ct);

    Task<Course?> GetByIdAsync(int id, CancellationToken ct);

    Task<Course?> GetByCodeAsync(string code, CancellationToken ct);

    Task AddAsync(Course course, CancellationToken ct);

    Task UpdateAsync(Course course, CancellationToken ct);

    Task DeleteAsync(Course course, CancellationToken ct);

    Task<bool> CodeExistsAsync(string code, CancellationToken ct);
    Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(
        PagedRequest request,
        CancellationToken ct
    );
}