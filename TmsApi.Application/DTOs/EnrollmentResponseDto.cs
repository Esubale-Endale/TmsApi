namespace TmsApi.Application.DTOs;

public record EnrollmentResponseDto(
    int Id,
    int StudentId,
    int CourseId,
    DateTime EnrolledAt
);