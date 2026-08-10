using MediatR;
using TmsApi.Application.DTOs;

namespace TmsApi.Application.Enrollments.Queries.GetEnrollmentById;
public record GetEnrollmentByIdQuery(int Id) : IRequest<EnrollmentResponseDto>;