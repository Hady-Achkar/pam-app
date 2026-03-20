namespace backend.Repositories;

using backend.Entities;
public interface IDentistRepository
{
    Task<List<Dentist>> GetAllAsync(CancellationToken ct = default);
    Task<Dentist?> GetByIdAsync(Guid id, CancellationToken ct = default);
}