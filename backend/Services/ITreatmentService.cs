namespace backend.Services;

using backend.DTOs;
public interface ITreatmentService
{
    Task<List<TreatmentResponse>> GetAllAsync(CancellationToken ct = default);
}
