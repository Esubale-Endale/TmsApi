using MediatR;
using TmsApi.Application.Common;

namespace TmsApi.Application.Courses.Commands.CreateCourse;

public record CreateCourseCommand(string Title, string Code, int MaxCapacity) : IRequest<Result<CourseCreated, CourseError>>;
public record CourseCreated(int CourseId, string Title, string Code, int MaxCapacity);