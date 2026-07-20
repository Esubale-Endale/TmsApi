
namespace TmsApi.Application.DTOs;

public record CreateEnrollmentRequest(
    int StudentId,
    int CourseId
);