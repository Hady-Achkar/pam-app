namespace backend.Services;

using backend.DTOs;
public interface IAppointmentService
{
    Task<AppointmentResponse> CreateAsync(CreateAppointmentRequest request, CancellationToken ct = default);
    Task<List<AppointmentResponse>> GetByPatientIdAsync(Guid patientId, CancellationToken ct = default);
}
