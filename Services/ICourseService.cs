using TmsApi.Entities;
using TmsApi.Dtos;
namespace TmsApi.Services;


public interface ICourseService
{
    Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct);
    Task<bool> CodeExistsAsync(string Code, CancellationToken ct);
    Task<IEnumerable<CourseResponseDto>> GetAllAsync(CancellationToken ct);
    Task<bool> DeleteAsync(int id);
}