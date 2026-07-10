using Tms.Api.Dtos;
using Tms.Api.Entities;

namespace Tms.Api.Services;

public interface IStudentService
{
    Task<StudentResponseDto> CreateAsync(CreateStudentRequest request,CancellationToken ct);
    Task<StudentResponseDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Student>> GetAllAsync(int page = 1, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id);
}
