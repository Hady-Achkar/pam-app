namespace backend.Mappings;

using backend.DTOs;
using backend.Entities;

public static class MappingExtensions
{
    public static PatientResponse ToResponse(this Patient patient) => new(
        patient.Id,
        patient.FullName,
        patient.Address,
        patient.PhotoUrl,
        patient.Appointments
            .OrderByDescending(a => a.ScheduledAt)
            .Select(a => a.ToResponse())
            .ToList());

    public static PatientSummaryResponse ToSummaryResponse(this Patient patient) => new(
        patient.Id,
        patient.FullName,
        patient.Address,
        patient.PhotoUrl,
        patient.Appointments.Count);

    public static AppointmentResponse ToResponse(this Appointment appointment) => new(
        appointment.Id,
        appointment.ScheduledAt,
        appointment.Dentist.ToResponse(),
        appointment.Treatment.ToResponse());

    public static DentistResponse ToResponse(this Dentist dentist) => new(
        dentist.Id,
        dentist.Name);

    public static TreatmentResponse ToResponse(this Treatment treatment) => new(
        treatment.Id,
        treatment.Name,
        treatment.DurationMinutes);
}
