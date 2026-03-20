namespace backend.Services;

using backend.DTOs;
using backend.Mappings;
using backend.Repositories;

public class DentistService(IDentistRepository repository) : IDentistService
{
    public async Task<List<DentistResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var dentists = await repository.GetAllAsync(ct);
        return [.. dentists.Select(d => d.ToResponse())];
    }
}
