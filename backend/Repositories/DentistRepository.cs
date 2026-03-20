namespace backend.Repositories;

using backend.Data;
using backend.Entities;
public class DentistRepository(AppDbContext context) : IDentistRepository
{
    public async Task<List<Dentist>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Dentists
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .ToListAsync(ct);
    }

    public async Task<Dentist?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Dentists.FindAsync([id], ct);
    }
}