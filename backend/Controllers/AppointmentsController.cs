namespace backend.Controllers;

using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AppointmentsController(IAppointmentService service) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentResponse>> Create(CreateAppointmentRequest request, CancellationToken ct)
    {
        var appointment = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetByPatientId), new { patientId = request.PatientId }, appointment);
    }

    [HttpGet("{patientId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AppointmentResponse>>> GetByPatientId(Guid patientId, CancellationToken ct)
    {
        var appointments = await service.GetByPatientIdAsync(patientId, ct);
        return Ok(appointments);
    }
}
