namespace Tms.Api.Dtos;

public record EnrollmentResponseDto(
    int Id,
    int StudentId,
    int CourseId,
    DateTime EnrolledAt
);