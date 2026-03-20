namespace backend.Storage;

public interface IPhotoStorage
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
}
