namespace backend.DTOs;

// record types are immutable, ideal for response DTOs
public record AppointmentResponse(
    Guid Id,
    DateTime ScheduledAt,
    DentistResponse Dentist,
    TreatmentResponse Treatment);
