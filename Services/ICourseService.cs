using Tms.Api.Entities;
using Tms.Api.Dtos;
namespace Tms.Api.Services;

public interface ICourseService
{
    Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct);
    Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> CodeExistsAsync(string Code, CancellationToken ct);
    Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(PagedRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id);
}