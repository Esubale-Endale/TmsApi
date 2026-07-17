using TmsApi.Application.DTOs;
using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface IStudentService
{
    Task<StudentResponseDto> CreateAsync(CreateStudentRequest request,CancellationToken ct);
    Task<StudentResponseDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> RegistrationNumberExistsAsync(string registrationNumber, CancellationToken ct);
    Task<PagedResponse<StudentResponseDto>> GetStudentsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id);
}
