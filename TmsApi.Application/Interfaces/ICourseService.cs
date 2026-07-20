using TmsApi.Application.DTOs;
using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface ICourseService
{
    Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct);
    Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<CourseResponseDto?> GetByCodeAsync(string code, CancellationToken ct);

    Task<Course?> GetCourseEntityByCodeAsync(string code, CancellationToken ct);
    Task<bool> CodeExistsAsync(string Code, CancellationToken ct);
    Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(PagedRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id);
}