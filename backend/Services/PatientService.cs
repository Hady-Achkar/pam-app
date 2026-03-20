namespace backend.Services;

using backend.DTOs;
using backend.Entities;
using backend.Mappings;
using backend.Repositories;
using backend.Storage;

public class PatientService(IPatientRepository repository, IPhotoStorage photoStorage) : IPatientService
{
    // HashSet for O(1) search. doesn't really matter here just a good practice.
    private static readonly HashSet<string> AllowedExtensions =
        new([".jpg", ".jpeg", ".png", ".webp"], StringComparer.OrdinalIgnoreCase);

    // identify the true file type by its signature (magic bytes)
    // prevents spoofing (e.g. renaming .exe to .jpg)
    private static readonly Dictionary<string, byte[]> FileSignatures = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = [0xFF, 0xD8, 0xFF],                        
        [".png"] = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A],
        [".webp"] = [0x52, 0x49, 0x46, 0x46]
    };
    private const int WebpHeaderSize = 12;
    private static readonly byte[] WebpMarker = [0x57, 0x45, 0x42, 0x50];

    // assume 5 mb max for photos size - ideally configurable
    private const long MaxPhotoSizeBytes = 5 * 1024 * 1024;

    public async Task<List<PatientSummaryResponse>> GetAllAsync(
        string? search, CancellationToken ct = default)
    {
        var patients = await repository.GetAllAsync(search, ct);
        return [.. patients.Select(p => p.ToSummaryResponse())];
    }

    public async Task<PatientResponse> CreateAsync(CreatePatientRequest request, CancellationToken ct = default)
    {
        string? photoUrl = null;

        if (request.Photo is not null)
        {
            if (request.Photo.Length == 0)
                throw new ArgumentException("Photo should not be empty.");

            if (request.Photo.Length > MaxPhotoSizeBytes)
                throw new ArgumentException("Photo should not exceed 5 MB.");

            // never trust filenames from client, only extract the extention
            var untrustedExtension = Path.GetExtension(request.Photo.FileName).ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(untrustedExtension) || !AllowedExtensions.Contains(untrustedExtension))
                throw new ArgumentException("Invalid photo format. Allowed extensions: .jpg, .jpeg, .png, .webp.");

            // normalize .jpeg to .jpg since they have the same signature - prevents duplicates
            var normalizedExtension = untrustedExtension is ".jpeg" ? ".jpg" : untrustedExtension;

            // verify the file signature
            await ValidateFileSignatureAsync(request.Photo, normalizedExtension, ct);

            // generate a random filename - don't leak original file names
            var fileName = Path.ChangeExtension(Path.GetRandomFileName(), normalizedExtension);

            // delegate to IPhotoStorage — swappable between local disk and cloud (e.g. Azure Blob)
            using var uploadStream = request.Photo.OpenReadStream();
            photoUrl = await photoStorage.UploadAsync(uploadStream, fileName, request.Photo.ContentType, ct);
        }

        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Address = request.Address.Trim(),
            PhotoUrl = photoUrl
        };

        var created = await repository.CreateAsync(patient, ct);

        return created.ToResponse();
    }

    public async Task<PatientResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var patient = await repository.GetByIdWithAppointmentsAsync(id, ct) 
        ?? throw new KeyNotFoundException($"Patient with ID {id} was not found.");

        return patient.ToResponse();
    }

    private static async Task ValidateFileSignatureAsync(IFormFile photo, string extension, CancellationToken ct)
    {
        var signature = FileSignatures[extension];
        var headerSize = extension is ".webp" ? WebpHeaderSize : signature.Length;
        var header = new byte[headerSize];

        using var stream = photo.OpenReadStream();
        var bytesRead = await stream.ReadAsync(header.AsMemory(0, headerSize), ct);

        if (bytesRead < signature.Length || !header.AsSpan(0, signature.Length).SequenceEqual(signature))
            throw new ArgumentException("Photo content does not match its file extension.");

        if (extension is ".webp" &&
            (bytesRead < WebpHeaderSize || !header.AsSpan(8, 4).SequenceEqual(WebpMarker)))
            throw new ArgumentException("Photo content does not match its file extension.");
    }
}
