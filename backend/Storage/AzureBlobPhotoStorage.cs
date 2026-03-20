namespace backend.Storage;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

// assumption: production uses Azure Blob Storage for scalable, CDN-friendly photo hosting.
// container is created on first upload if it doesn't exist.
public class AzureBlobPhotoStorage(BlobServiceClient blobServiceClient) : IPhotoStorage
{
    private const string ContainerName = "photos";

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        }, ct);

        // return the full url — served directly by azure cdn
        return blobClient.Uri.AbsoluteUri;
    }
}
