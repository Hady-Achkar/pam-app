namespace backend.Repositories;

using backend.Data;
using backend.Entities;
public class PatientRepository(AppDbContext context) : IPatientRepository
{
    public async Task<List<Patient>> GetAllAsync(
        string? search, CancellationToken ct = default)
    {
        var query = context.Patients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.FullName.Contains(search));

        return await query
            .Include(p => p.Appointments)
            .OrderBy(p => p.FullName)
            .ToListAsync(ct);
    }

    public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Patients.FindAsync([id], ct);
    }

    public async Task<Patient?> GetByIdWithAppointmentsAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Patients
            .Include(p => p.Appointments)
                .ThenInclude(a => a.Dentist)
            .Include(p => p.Appointments)
                .ThenInclude(a => a.Treatment)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Patient> CreateAsync(Patient patient, CancellationToken ct = default)
    {
        context.Patients.Add(patient);
        await context.SaveChangesAsync(ct);
        return patient;
    }
}
