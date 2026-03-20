namespace backend.Controllers;

using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TreatmentsController(ITreatmentService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TreatmentResponse>>> GetAll(CancellationToken ct)
    {
        var treatments = await service.GetAllAsync(ct);
        return Ok(treatments);
    }
}
