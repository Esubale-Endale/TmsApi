using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface ICertificateRepository
{
    Task<List<Certificate>> GetAllAsync(CancellationToken cancellationToken);
    Task<Certificate?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Certificate certificate, CancellationToken cancellationToken);
    Task UpdateAsync(Certificate certificate, CancellationToken cancellationToken);
    Task DeleteAsync(Certificate certificate, CancellationToken cancellationToken);
}