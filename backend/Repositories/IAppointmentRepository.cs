namespace backend.Repositories;

using backend.Entities;
public interface IAppointmentRepository
{
    Task<List<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken ct = default);
    Task<bool> HasConflictAsync(Guid dentistId, Guid patientId, DateTime newStart, DateTime newEnd, CancellationToken ct = default);
    Task<Appointment> CreateAsync(Appointment appointment, CancellationToken ct = default);
}