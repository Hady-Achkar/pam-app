namespace backend.Services;

using backend.DTOs;
using backend.Mappings;
using backend.Repositories;

public class TreatmentService(ITreatmentRepository repository) : ITreatmentService
{
    public async Task<List<TreatmentResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var treatments = await repository.GetAllAsync(ct);
        return [.. treatments.Select(t => t.ToResponse())];
    }
}
