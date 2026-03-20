using backend.DTOs;
using backend.Entities;
using backend.Repositories;
using backend.Services;
using Moq;

public class AppointmentServiceTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepoMock = new();
    private readonly Mock<IPatientRepository> _patientRepoMock = new();
    private readonly Mock<IDentistRepository> _dentistRepoMock = new();
    private readonly Mock<ITreatmentRepository> _treatmentRepoMock = new();
    private readonly AppointmentService _sut;

    private readonly Patient _testPatient = new()
    {
        Id = Guid.NewGuid(),
        FullName = "John Doe",
        Address = "123 Main St"
    };

    private readonly Dentist _testDentist = new()
    {
        Id = Guid.NewGuid(),
        Name = "Dr. John Doe"
    };

    private readonly Treatment _testTreatment = new()
    {
        Id = Guid.NewGuid(),
        Name = "Cleaning",
        DurationMinutes = 30
    };

    public AppointmentServiceTests()
    {
        _sut = new AppointmentService(
            _appointmentRepoMock.Object,
            _patientRepoMock.Object,
            _dentistRepoMock.Object,
            _treatmentRepoMock.Object);
    }

    private void SetupAllReposValid()
    {
        _patientRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testPatient);

        _dentistRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testDentist);

        _treatmentRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testTreatment);

        _appointmentRepoMock
            .Setup(r => r.HasConflictAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _appointmentRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, CancellationToken _) => a);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsAppointmentResponse()
    {
        SetupAllReposValid();

        var request = new CreateAppointmentRequest
        {
            PatientId = _testPatient.Id,
            DentistId = _testDentist.Id,
            TreatmentId = _testTreatment.Id,
            ScheduledAt = DateTime.UtcNow.AddDays(1)
        };

        var result = await _sut.CreateAsync(request);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Dr. John Doe", result.Dentist.Name);
        Assert.Equal("Cleaning", result.Treatment.Name);
        Assert.Equal(30, result.Treatment.DurationMinutes);
    }

    [Fact]
    public async Task CreateAsync_PatientNotFound_ThrowsKeyNotFoundException()
    {
        _patientRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient?)null);

        var request = new CreateAppointmentRequest
        {
            PatientId = Guid.NewGuid(),
            DentistId = _testDentist.Id,
            TreatmentId = _testTreatment.Id,
            ScheduledAt = DateTime.UtcNow.AddDays(1)
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_DentistNotFound_ThrowsKeyNotFoundException()
    {
        _patientRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testPatient);

        _dentistRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dentist?)null);

        var request = new CreateAppointmentRequest
        {
            PatientId = _testPatient.Id,
            DentistId = Guid.NewGuid(),
            TreatmentId = _testTreatment.Id,
            ScheduledAt = DateTime.UtcNow.AddDays(1)
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_TreatmentNotFound_ThrowsKeyNotFoundException()
    {
        _patientRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testPatient);

        _dentistRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testDentist);

        _treatmentRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Treatment?)null);

        var request = new CreateAppointmentRequest
        {
            PatientId = _testPatient.Id,
            DentistId = _testDentist.Id,
            TreatmentId = Guid.NewGuid(),
            ScheduledAt = DateTime.UtcNow.AddDays(1)
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_DentistConflict_ThrowsInvalidOperationException()
    {
        SetupAllReposValid();

        _appointmentRepoMock
            .Setup(r => r.HasConflictAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateAppointmentRequest
        {
            PatientId = _testPatient.Id,
            DentistId = _testDentist.Id,
            TreatmentId = _testTreatment.Id,
            ScheduledAt = DateTime.UtcNow.AddDays(1)
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(request));
        Assert.Contains("already has an appointment", ex.Message);
    }

    [Fact]
    public async Task GetByPatientIdAsync_ReturnsMappedList()
    {
        var appointments = new List<Appointment>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ScheduledAt = DateTime.UtcNow.AddDays(2),
                PatientId = _testPatient.Id,
                Dentist = _testDentist,
                Treatment = _testTreatment
            },
            new()
            {
                Id = Guid.NewGuid(),
                ScheduledAt = DateTime.UtcNow.AddDays(1),
                PatientId = _testPatient.Id,
                Dentist = _testDentist,
                Treatment = _testTreatment
            }
        };

        _appointmentRepoMock
            .Setup(r => r.GetByPatientIdAsync(_testPatient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointments);

        var result = await _sut.GetByPatientIdAsync(_testPatient.Id);

        Assert.Equal(2, result.Count);
        Assert.True(result[0].ScheduledAt > result[1].ScheduledAt);
        Assert.All(result, r =>
        {
            Assert.Equal("Dr. John Doe", r.Dentist.Name);
            Assert.Equal("Cleaning", r.Treatment.Name);
        });
    }
}
