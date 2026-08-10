using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync(CancellationToken cancellationToken);
    Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Student student, CancellationToken cancellationToken);
    Task UpdateAsync(Student student, CancellationToken cancellationToken);
    Task DeleteAsync(Student student, CancellationToken cancellationToken);
}