using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces; 
public interface   ICachedCourseService
{
    Task<Course> GetCourseAsync(string code, CancellationToken ct);
    Task<List<Course>> GetAllCoursesAsync(CancellationToken ct);
    Task InvalidateCourseCacheAsync(CancellationToken ct);

} 