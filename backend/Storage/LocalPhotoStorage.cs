namespace backend.Storage;

// local disk storage for development — no cloud provider needed.
// files saved under ContentRootPath/uploads/photos, served via PhotosController.
// unprotected for now, no auth in place.
public class LocalPhotoStorage(IWebHostEnvironment env) : IPhotoStorage
{
    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var uploadDir = Path.Combine(env.ContentRootPath, "uploads", "photos");
        Directory.CreateDirectory(uploadDir);

        var filePath = Path.Combine(uploadDir, fileName);
        await using var fileStream = File.Create(filePath);
        await stream.CopyToAsync(fileStream, ct);

        /*
         - store the api url, fetch via photos controller
         - ideal for caching and CDN
         - decouples storage from the patient service
         - allows for plug and play for different storage solutions
        */
        return $"/api/photos/{fileName}";
    }
}
