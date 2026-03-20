namespace backend.Repositories;

using backend.Data;
using backend.Entities;
public class TreatmentRepository(AppDbContext context) : ITreatmentRepository
{
    public async Task<List<Treatment>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Treatments
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync(ct);
    }

    public async Task<Treatment?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Treatments.FindAsync([id], ct);
    }
}
