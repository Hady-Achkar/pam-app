namespace backend.DTOs;

public record PatientResponse(
    Guid Id,
    string FullName,
    string Address,
    string? PhotoUrl,
    List<AppointmentResponse> Appointments);
