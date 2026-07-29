using MediatR;
using TmsApi.Application.DTOs;

namespace TmsApi.Application.Students.Queries.GetStudentById;

public record GetStudentByIdQuery(int Id) : IRequest<StudentResponseDto?>;

