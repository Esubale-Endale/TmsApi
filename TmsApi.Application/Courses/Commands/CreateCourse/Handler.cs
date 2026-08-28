using MediatR;
using TmsApi.Application.Common;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;

namespace TmsApi.Application.Courses.Commands.CreateCourse;

public class CreateCourseHandler(ICourseRepository repo)
    : IRequestHandler<CreateCourseCommand, Result<CourseCreated, CourseError>>
{
    public async Task<Result<CourseCreated, CourseError>> Handle(CreateCourseCommand command, CancellationToken ct)
    {
        var codeExists = await repo.CodeExistsAsync(command.Code, ct);
        if (codeExists)
            return Result<CourseCreated, CourseError>.Failure(CourseError.AlreadyCreated(command.Code));

        var course = new Course
        {
            Title = command.Title,
            Code = command.Code,
            MaxCapacity = command.MaxCapacity
        };

        await repo.AddAsync(course, ct);

        return Result<CourseCreated, CourseError>.Success(new CourseCreated(course.Id, course.Title, course.Code, course.MaxCapacity));
    }
}