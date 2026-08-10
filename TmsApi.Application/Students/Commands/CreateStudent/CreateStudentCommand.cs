using MediatR;

namespace TmsApi.Application.Students.Commands.CreateStudent;

public record CreateStudentCommand(
    string RegistrationNumber,
    string Name,
    decimal GPA,
    bool IsActived
) : IRequest;