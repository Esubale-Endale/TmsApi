using System.ComponentModel.DataAnnotations;

namespace TmsApi.Dtos;

public sealed class CreateCourseRequest
{
    [Required]
    [StringLength(10)]
    public string Code { get; init; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string Title { get; init; } = string.Empty;

    [Range(1, 500)]
    public int MaxCapacity { get; init; }
}