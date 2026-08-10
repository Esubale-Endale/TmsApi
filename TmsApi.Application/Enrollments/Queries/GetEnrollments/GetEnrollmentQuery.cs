using MediatR;
using TmsApi.Application.DTOs;

namespace TmsApi.Application.Enrollments.Queries.GetEnrollments;

public record GetEnrollmentsQuery : IRequest<List<EnrollmentResponseDto>>;