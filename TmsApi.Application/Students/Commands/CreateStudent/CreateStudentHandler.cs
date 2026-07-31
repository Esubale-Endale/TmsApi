using MediatR;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;

namespace TmsApi.Application.Students.Commands.CreateStudent;

public class CreateStudentHandler(
    IStudentRepository repository)
    : IRequestHandler<CreateStudentCommand>
{
    public async Task Handle(CreateStudentCommand request, CancellationToken ct)
    {
        var student = new Student
        {
            RegistrationNumber = request.RegistrationNumber,
            Name = request.Name,
            GPA = request.GPA,
            IsActived = request.IsActived,
        };

        await repository.AddAsync(student, ct);
    }
}