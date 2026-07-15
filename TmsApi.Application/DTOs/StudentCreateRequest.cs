using System.ComponentModel.DataAnnotations;

namespace TmsApi.Application.DTOs;

public record CreateStudentRequest
{
    [Required, MaxLength(20)]
    public required string RegistrationNumber { get; init; }

    [Required, MaxLength(200)]
    public required string Name { get; init; }

    [Range(0,4)]
    public decimal GPA { get; init; }

    public bool IsActive { get; init; }
}