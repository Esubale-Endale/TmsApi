namespace TmsApi.Dtos;

public sealed class CourseResponseDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public int MaxCapacity { get; init; }
    public int EnrollmentCount { get; init; }
}