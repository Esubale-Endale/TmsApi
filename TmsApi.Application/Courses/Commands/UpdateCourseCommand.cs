using MediatR;

namespace TmsApi.Application.Courses.Commands;
public class UpdateCourseCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Code { get; set; }
    public int MaxCapacity { get; set; }

}