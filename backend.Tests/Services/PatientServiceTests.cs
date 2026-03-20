using backend.DTOs;
using backend.Entities;
using backend.Mappings;
using backend.Repositories;
using backend.Services;
using backend.Storage;
using Microsoft.AspNetCore.Http;
using Moq;

public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _repoMock = new();
    private readonly Mock<IPhotoStorage> _storageMock = new();
    private readonly PatientService _sut;

    public PatientServiceTests()
    {
        _sut = new PatientService(_repoMock.Object, _storageMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_NoPhoto_ReturnsPatientResponse()
    {
        var request = new CreatePatientRequest
        {
            FullName = "  John Doe  ",
            Address = " 123 Main St "
        };

        _repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient p, CancellationToken _) => p);

        var result = await _sut.CreateAsync(request);

        Assert.Equal("John Doe", result.FullName);
        Assert.Equal("123 Main St", result.Address);
        Assert.Null(result.PhotoUrl);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task CreateAsync_InvalidPhotoExtension_ThrowsArgumentException()
    {
        var request = new CreatePatientRequest
        {
            FullName = "Jane Doe",
            Address = "123 qwe",
            Photo = CreateFormFile(
                [0x25, 0x50, 0x44, 0x46],
                "document.pdf",
                "application/pdf")
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_PhotoExceedsMaxSize_ThrowsArgumentException()
    {
        var request = new CreatePatientRequest
        {
            FullName = "Jane Doe",
            Address = "qwe 123",
            Photo = CreateFormFile(
                new byte[6 * 1024 * 1024],
                "photo.jpg",
                "image/jpeg")
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_SpoofedImageContent_ThrowsArgumentException()
    {
        var request = new CreatePatientRequest
        {
            FullName = "Jane Doe",
            Address = "address",
            Photo = CreateFormFile(
                "<script>alert('xss')</script>"u8.ToArray(),
                "payload.png",
                "image/png")
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_UnsupportedPhotoExtension_ThrowsArgumentException()
    {
        var request = new CreatePatientRequest
        {
            FullName = "Jane Doe",
            Address = "456 Oak Ave",
            Photo = CreateFormFile(
                [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A],
                "photo.txt",
                "image/png")
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_ValidPng_DelegatesToPhotoStorage()
    {
        _repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient p, CancellationToken _) => p);

        _storageMock
            .Setup(s => s.UploadAsync(It.IsAny<Stream>(), It.Is<string>(f => f.EndsWith(".png")),
                "image/png", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Stream _, string fileName, string _, CancellationToken _) =>
                $"/api/photos/{fileName}");

        var request = new CreatePatientRequest
        {
            FullName = "Jane Doe",
            Address = "456 Oak Ave",
            Photo = CreateFormFile(
                [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x00],
                "photo.png",
                "image/png")
        };

        var result = await _sut.CreateAsync(request);

        Assert.NotNull(result.PhotoUrl);
        Assert.EndsWith(".png", result.PhotoUrl, StringComparison.OrdinalIgnoreCase);
        _storageMock.Verify(s => s.UploadAsync(
            It.IsAny<Stream>(), It.Is<string>(f => f.EndsWith(".png")),
            "image/png", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_PatientExists_ReturnsPatientResponse()
    {
        var patientId = Guid.NewGuid();
        var patient = new Patient
        {
            Id = patientId,
            FullName = "John Doe",
            Address = "123 Main St",
            Appointments =
            [
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    ScheduledAt = DateTime.UtcNow.AddDays(1),
                    Dentist = new Dentist
                    {
                        Id = Guid.NewGuid(),
                        Name = "Dr. John Doe"
                    },
                    Treatment = new Treatment
                    {
                        Id = Guid.NewGuid(),
                        Name = "Cleaning",
                        DurationMinutes = 30
                    }
                },
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    ScheduledAt = DateTime.UtcNow.AddDays(3),
                    Dentist = new Dentist
                    {
                        Id = Guid.NewGuid(),
                        Name = "Dr. Jane Doe"
                    },
                    Treatment = new Treatment
                    {
                        Id = Guid.NewGuid(),
                        Name = "Root Canal",
                        DurationMinutes = 90
                    }
                }
            ]
        };

        _repoMock
            .Setup(r => r.GetByIdWithAppointmentsAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        var result = await _sut.GetByIdAsync(patientId);

        Assert.Equal(patientId, result.Id);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal(2, result.Appointments.Count);
        Assert.True(result.Appointments[0].ScheduledAt > result.Appointments[1].ScheduledAt);
    }

    [Fact]
    public async Task GetByIdAsync_PatientNotFound_ThrowsKeyNotFoundException()
    {
        var patientId = Guid.NewGuid();

        _repoMock
            .Setup(r => r.GetByIdWithAppointmentsAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient?)null);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetByIdAsync(patientId));
        Assert.Contains(patientId.ToString(), ex.Message);
    }

    private static IFormFile CreateFormFile(byte[] content, string fileName, string contentType)
    {
        var stream = new MemoryStream(content);

        return new FormFile(stream, 0, content.Length, "Photo", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}
