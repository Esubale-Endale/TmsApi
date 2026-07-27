using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Caching;
using TmsApi.Application.Common.Exceptions;
namespace TmsApi.Infrastructure.Services;

public class CachedCourseService(HybridCache cache, ICourseRepository repo, ILogger<CachedCourseService> logger) : ICachedCourseService
{
    public async Task<Course> GetCourseAsync(string code,
CancellationToken ct)
    {
        var key = CacheKeys.Course(code);
        var dbHit = false;

        var dto = await cache.GetOrCreateAsync(
            key,
            (repo, code),
            async (state, token) =>
            {
                dbHit = true;
                logger.LogInformation("Cache MISS for {Key}  fetching from DB", key); 
                var course = await state.repo.GetByCodeAsync(state.code, token)
                    ?? throw new NotFoundException($"Course {state.code} not found.");
                return course;
            },
            tags: [CacheKeys.CoursesTag],
            cancellationToken: ct);

        if (!dbHit)
            logger.LogInformation("Cache HIT for {Key}", key);

        return dto;
    }

    public async Task<List<Course>> GetAllCoursesAsync(CancellationToken ct)
    {
        var key = CacheKeys.CoursesAll;
        var dbHit = false;

        var list = await cache.GetOrCreateAsync(
            key(),
            repo,
            async (state, token) =>
            {
                dbHit = true;
                logger.LogInformation("Cache MISS for {Key}  fetching from DB", key); 
                var courses = await state.GetAllAsync(token);
                return courses.ToList();
            },
            tags: [CacheKeys.CoursesTag],
            cancellationToken: ct);

        if (!dbHit)
            logger.LogInformation("Cache HIT for {Key}", key);

        return list;
    }

    public async Task InvalidateCourseCacheAsync(CancellationToken ct)
    {
        logger.LogInformation("Invalidating cache tag {Tag}",
CacheKeys.CoursesTag);
        await cache.RemoveByTagAsync(CacheKeys.CoursesTag, ct);
    }
}