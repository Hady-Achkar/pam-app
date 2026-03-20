namespace backend.DTOs;

public record PatientSummaryResponse(
    Guid Id,
    string FullName,
    string Address,
    string? PhotoUrl,
    int AppointmentCount);
