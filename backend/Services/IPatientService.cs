namespace backend.Services;

using backend.DTOs;
public interface IPatientService
{
    Task<List<PatientSummaryResponse>> GetAllAsync(string? search, CancellationToken ct = default);
    Task<PatientResponse> CreateAsync(CreatePatientRequest request, CancellationToken ct = default);
    Task<PatientResponse> GetByIdAsync(Guid id, CancellationToken ct = default);
}
