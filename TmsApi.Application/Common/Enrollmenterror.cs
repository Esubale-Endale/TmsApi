namespace TmsApi.Application.Common;

public sealed record EnrollmentError(string Code, string Message){

    public static EnrollmentError CourseNotFound(string code) => new("course_not_found", $"Course {code} not found.");
    public static EnrollmentError CourseFull(string code,int capacity) => new("course_full", $"Course {code} is full (Capacity: {capacity}).");
    public static EnrollmentError AlreadyEnrolled(string code, string studentId) => new("already_enrolled", $"Student {studentId} is already Enrolled in course {code}.");
    
}