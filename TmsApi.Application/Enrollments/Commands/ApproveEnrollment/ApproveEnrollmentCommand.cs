using MediatR;

namespace TmsApi.Application.Enrollments.Commands.ApproveEnrollment;

public record ApproveEnrollmentCommand(int EnrollmentId) : IRequest;