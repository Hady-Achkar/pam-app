namespace backend.Repositories;

using backend.Data;
using backend.Entities;
public class AppointmentRepository(AppDbContext context) : IAppointmentRepository
{
    public async Task<List<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken ct = default)
    {
        return await context.Appointments
            .Where(a => a.PatientId == patientId)
            .Include(a => a.Dentist)
            .Include(a => a.Treatment)
            .AsNoTracking()
            .OrderByDescending(a => a.ScheduledAt)
            .ToListAsync(ct);
    }

    public async Task<bool> HasConflictAsync(Guid dentistId, Guid patientId, DateTime scheduledAt, CancellationToken ct = default)
    {
        return await context.Appointments.AnyAsync(a =>
            a.ScheduledAt == scheduledAt
            && (a.DentistId == dentistId || a.PatientId == patientId),
            ct);
    }

    public async Task<Appointment> CreateAsync(Appointment appointment, CancellationToken ct = default)
    {
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync(ct);
        return appointment;
    }
}
