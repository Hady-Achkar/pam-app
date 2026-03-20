namespace backend.Repositories;

using backend.Entities;
public interface ITreatmentRepository
{
    Task<List<Treatment>> GetAllAsync(CancellationToken ct = default);
    Task<Treatment?> GetByIdAsync(Guid id, CancellationToken ct = default);
}