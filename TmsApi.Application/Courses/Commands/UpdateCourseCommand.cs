using MediatR;

namespace TmsApi.Application.Courses.Commands;

public class UpdateCourseCommand : IRequest<bool>
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Code { get; set; }
    public int MaxCapacity { get; set; }
}