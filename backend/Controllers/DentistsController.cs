namespace backend.Controllers;

using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DentistsController(IDentistService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DentistResponse>>> GetAll(CancellationToken ct)
    {
        var dentists = await service.GetAllAsync(ct);
        return Ok(dentists);
    }
}
