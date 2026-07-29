using MediatR;
using TmsApi.Application.DTOs;
using TmsApi.Application.Interfaces;

namespace TmsApi.Application.Students.Queries.GetStudents;

public class GetStudentsHandler(IStudentRepository repository) : IRequestHandler<GetStudentsQuery, List<StudentResponseDto>>
{
    public async Task<List<StudentResponseDto>> Handle(
    GetStudentsQuery request,
    CancellationToken cancellationToken)
    {
        var students = await repository.GetAllAsync(cancellationToken);
        return [.. students.Select(s => new StudentResponseDto(
        s.Id,
        s.RegistrationNumber,
        s.Name,
        s.GPA,
        s.IsActived,
        s.Enrollments.Count
        ))];
    }
}