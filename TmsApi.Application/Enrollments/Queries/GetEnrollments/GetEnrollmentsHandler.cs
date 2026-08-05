using MediatR;
using TmsApi.Application.DTOs;
using TmsApi.Application.Interfaces;

namespace TmsApi.Application.Enrollments.Queries.GetEnrollments;

public class GetEnrollmentsHandler(IEnrollmentRepository repository) : IRequestHandler<GetEnrollmentsQuery, List<EnrollmentResponseDto>>
{
    public async Task<List<EnrollmentResponseDto>> Handle(
        GetEnrollmentsQuery request,
        CancellationToken ct)
    {
        var enrollments = await repository.GetAllAsync(ct);

        return [.. enrollments.Select(e =>
            new EnrollmentResponseDto(
                e.Id,
                e.StudentId,
                e.CourseId,
                e.Status,
                e.EnrolledAt
            ))];
    }
}