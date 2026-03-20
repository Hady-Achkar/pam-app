namespace backend.Repositories;

using backend.Entities;
public interface IPatientRepository
{
    Task<List<Patient>> GetAllAsync(string? search, CancellationToken ct = default);
    Task<Patient?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Patient?> GetByIdWithAppointmentsAsync(Guid id, CancellationToken ct = default);
    Task<Patient> CreateAsync(Patient patient, CancellationToken ct = default);
}
