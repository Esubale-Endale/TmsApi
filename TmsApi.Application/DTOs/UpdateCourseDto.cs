
namespace TmsApi.Application.DTOs;

public class UpdateCourseDto
{
    public required string Title { get; set; }
    public required string Code { get; set; }
    public int MaxCapacity { get; set; }
    public string? InstructorId { get; set; }
}