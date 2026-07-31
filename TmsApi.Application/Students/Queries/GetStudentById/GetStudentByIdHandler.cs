using MediatR;
using TmsApi.Application.DTOs;
using TmsApi.Application.Interfaces;

namespace TmsApi.Application.Students.Queries.GetStudentById;

public class GetStudentById(IStudentRepository repository) : IRequestHandler<GetStudentByIdQuery, StudentResponseDto?>
{
    public async Task<StudentResponseDto?> Handle(GetStudentByIdQuery request, CancellationToken ct)
    {
        var student = await repository.GetByIdAsync(request.Id, ct);

        if (student == null)
            return null;

        return new StudentResponseDto(
            student.Id,
            student.RegistrationNumber,
            student.Name,
            student.GPA,
            student.IsActived,
            student.Enrollments.Count
            );
    }
}


