using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface IAssessmentRepository
{
    Task<List<Assessment>> GetAllAsync(CancellationToken cancellationToken);
    Task<Assessment?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Assessment assessment, CancellationToken cancellationToken);
    Task UpdateAsync(Assessment assessment, CancellationToken cancellationToken);
    Task DeleteAsync(Assessment assessment, CancellationToken cancellationToken);
}