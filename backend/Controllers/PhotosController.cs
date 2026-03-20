namespace backend.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

[ApiController]
[Route("api/[controller]")]
public class PhotosController(IWebHostEnvironment env) : ControllerBase
{
    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

    [HttpGet("{fileName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(string fileName)
    {
        var sanitized = Path.GetFileName(fileName);
        var filePath = Path.Combine(env.ContentRootPath, "uploads", "photos", sanitized);

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        // determine content type based on file extension, default to application/octet-stream if unknown
        if (!ContentTypeProvider.TryGetContentType(fileName, out var contentType))
            contentType = "application/octet-stream";

        return PhysicalFile(filePath, contentType);
    }
}
