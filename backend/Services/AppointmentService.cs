namespace backend.Services;

using backend.DTOs;
using backend.Entities;
using backend.Mappings;
using backend.Repositories;

public class AppointmentService(
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IDentistRepository dentistRepository,
    ITreatmentRepository treatmentRepository) : IAppointmentService
{
    public async Task<AppointmentResponse> CreateAsync(CreateAppointmentRequest request, CancellationToken ct = default)
    {
        _ = await patientRepository.GetByIdAsync(request.PatientId, ct)
            ?? throw new KeyNotFoundException("Patient not found.");

        var dentist = await dentistRepository.GetByIdAsync(request.DentistId, ct)
            ?? throw new KeyNotFoundException("Dentist not found.");

        var treatment = await treatmentRepository.GetByIdAsync(request.TreatmentId, ct)
            ?? throw new KeyNotFoundException("Treatment not found.");

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            ScheduledAt = request.ScheduledAt,
            PatientId = request.PatientId,
            DentistId = dentist.Id,
            TreatmentId = treatment.Id
        };

        await appointmentRepository.CreateAsync(appointment, ct);

        // just assign, no extra db calls 
        appointment.Dentist = dentist;
        appointment.Treatment = treatment;

        return appointment.ToResponse();
    }

    public async Task<List<AppointmentResponse>> GetByPatientIdAsync(Guid patientId, CancellationToken ct = default)
    {
        var appointments = await appointmentRepository.GetByPatientIdAsync(patientId, ct);
        return appointments.Select(a => a.ToResponse()).ToList();
    }
}
