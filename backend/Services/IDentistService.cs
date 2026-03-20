namespace backend.Services;

using backend.DTOs;
public interface IDentistService
{
    Task<List<DentistResponse>> GetAllAsync(CancellationToken ct = default);
}
