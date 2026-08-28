namespace TmsApi.Application.Common;

public sealed record CourseError(string Code, string Message)
{

    public static CourseError CourseNotFound(string code) => new("course_not_found", $"Course {code} not found.");
    public static CourseError CourseFull(string code, int capacity) => new("course_full", $"Course {code} is full (Capacity: {capacity}).");
    public static CourseError AlreadyCreated(string code) => new("already_created", $"Course {code} already exists.");

}