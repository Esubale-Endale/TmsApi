using MediatR;
using TmsApi.Application.Interfaces;

namespace TmsApi.Application.Courses.Command;

public class UpdateCourseHandler(
    ICourseService repo,
    ICachedCourseService cachedService)
    : IRequestHandler<UpdateCourseCommand, bool>
{
    public async Task<bool> Handle(UpdateCourseCommand command,
        CancellationToken ct)
    {
        await repo.UpdateAsync(command, ct);
        await cachedService.InvalidateCourseCacheAsync(ct);
        return true;
    }
}