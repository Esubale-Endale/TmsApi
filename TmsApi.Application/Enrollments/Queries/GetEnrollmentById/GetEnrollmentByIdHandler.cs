using MediatR;
using TmsApi.Application.DTOs;
using TmsApi.Application.Interfaces;

namespace TmsApi.Application.Enrollments.Queries.GetEnrollmentById;

public class GetEnrollmentByIdHandler(IEnrollmentRepository repository) : IRequestHandler<GetEnrollmentByIdQuery, EnrollmentResponseDto>
{
    public async Task<EnrollmentResponseDto> Handle(GetEnrollmentByIdQuery request, CancellationToken ct)
    {
        var enrollment = await repository.GetByIdAsync(request.Id, ct) ?? throw new KeyNotFoundException($"Enrollment with ID {request.Id} not found.");

        return new EnrollmentResponseDto(
            enrollment.Id,
            enrollment.StudentId,
            enrollment.CourseId,
            enrollment.Status,
            enrollment.EnrolledAt
        );
    }
}