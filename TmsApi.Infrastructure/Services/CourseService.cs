using Microsoft.Extensions.Logging;
using TmsApi.Application.Courses.Command;
using TmsApi.Domain.Entities;
using TmsApi.Application.DTOs;
using TmsApi.Application.Interfaces;

namespace TmsApi.Infrastructure.Services;

public class CourseService(ICourseRepository repository, ILogger<CourseService> logger) : ICourseService
{
    public Task<bool> CodeExistsAsync(string code, CancellationToken ct)
    {
        return repository.CodeExistsAsync(code, ct);
    }

    public async Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var course = await repository.GetByIdAsync(id, ct);

        if (course is null)
            return null;

        return new CourseResponseDto(
            course.Id,
            course.Code,
            course.Title,
            course.MaxCapacity,
            course.Enrollments.Count);
    }

    public async Task<CourseResponseDto?> GetByCodeAsync(string code, CancellationToken ct)
    {
        var course = await repository.GetByCodeAsync(code, ct);

        if (course is null)
            return null;

        return new CourseResponseDto(
            course.Id,
            course.Code,
            course.Title,
            course.MaxCapacity,
            course.Enrollments.Count
        );
    }

    public Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(
    PagedRequest request,
    CancellationToken ct)
    {
        return repository.GetCoursesAsync(request, ct);
    }
    public Task<Course?> GetCourseEntityByCodeAsync(string code, CancellationToken ct)
    {
        return repository.GetByCodeAsync(code, ct);
    }
    public async Task<CourseResponseDto> CreateAsync(
     CreateCourseRequest request,
     CancellationToken ct)
    {
        var course = new Course
        {
            Code = request.Code,
            Title = request.Title,
            MaxCapacity = request.MaxCapacity
        };

        await repository.AddAsync(course, ct);

        logger.LogInformation(
            "Created course {CourseId} ({Code})",
            course.Id,
            course.Code);

        return new CourseResponseDto(
            course.Id,
            course.Code,
            course.Title,
            course.MaxCapacity,
            course.Enrollments.Count);
    }
    public async Task<bool> DeleteAsync(
       int id,
       CancellationToken ct)
    {
        var course = await repository.GetByIdAsync(id, ct);

        if (course is null)
        {
            logger.LogWarning(
                "Delete failed. Course {CourseId} not found",
                id);

            return false;
        }

        await repository.DeleteAsync(course, ct);

        logger.LogInformation(
            "Deleted course {CourseId}",
            id);

        return true;
    }
    public async Task UpdateAsync(UpdateCourseCommand command, CancellationToken ct)
    {
        var course = await repository.GetByIdAsync(command.Id, ct);


        if (course is null)
        {
            logger.LogWarning(
                "Update failed. Course {CourseId} not found",
                command.Id);

            throw new InvalidOperationException($"Course with ID {command.Id} not found.");
        }

        await repository.UpdateAsync(course, ct); ;

        logger.LogInformation(
            "Updated course {CourseId}",
            command.Id);
    }
}

