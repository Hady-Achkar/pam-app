namespace backend.Controllers;

using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PatientsController(IPatientService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PatientSummaryResponse>>> GetAll(
        string? search,
        CancellationToken ct = default)
    {
        var result = await service.GetAllAsync(search, ct);
        return Ok(result);
    }

    [HttpPost]
    [RequestSizeLimit(6 * 1024 * 1024)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientResponse>> Create([FromForm] CreatePatientRequest request, CancellationToken ct)
    {
        var patient = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientResponse>> GetById(Guid id, CancellationToken ct)
    {
        var patient = await service.GetByIdAsync(id, ct);
        return Ok(patient);
    }
}
