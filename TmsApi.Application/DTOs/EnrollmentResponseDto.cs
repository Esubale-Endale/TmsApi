using TmsApi.Domain.Entities;
using TmsApi.Domain.Enums;

namespace TmsApi.Application.DTOs;

public record EnrollmentResponseDto(
    int Id,
    int StudentId,
    int CourseId,
    EnrollmentStatus Status,
    DateTime EnrolledAt
);