using MediatR;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Enums;

namespace TmsApi.Application.Enrollments.Commands.ApproveEnrollment;

public class ApproveEnrollmentHandler(IEnrollmentRepository repository) : IRequestHandler<ApproveEnrollmentCommand>
{
    public async Task Handle(ApproveEnrollmentCommand request, CancellationToken ct)
    {
        var enrollment = await repository.GetByIdAsync(request.EnrollmentId, ct) ?? throw new KeyNotFoundException(
                $"Enrollment with ID {request.EnrollmentId} was not found.");

        if (enrollment.Status == EnrollmentStatus.Approved)
        {
            throw new InvalidOperationException(
                $"Enrollment {request.EnrollmentId} is already approved.");
        }

        enrollment.Status = EnrollmentStatus.Approved;
        await repository.UpdateAsync(enrollment, ct);
    }
}