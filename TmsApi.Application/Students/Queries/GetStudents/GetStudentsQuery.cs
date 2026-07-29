using MediatR;
using TmsApi.Application.DTOs;

namespace TmsApi.Application.Students.Queries.GetStudents;

public record GetStudentsQuery : IRequest<List<StudentResponseDto>>;