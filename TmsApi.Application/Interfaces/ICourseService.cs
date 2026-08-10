using TmsApi.Application.Courses.Commands;
using TmsApi.Application.DTOs;
using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface ICourseService
{
    Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct);
    Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<CourseResponseDto?> GetByCodeAsync(string code, CancellationToken ct);
    Task<Course?> GetCourseEntityByCodeAsync(string code, CancellationToken ct);
    Task UpdateAsync(UpdateCourseCommand command, CancellationToken ct);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct);
    Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(PagedRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}