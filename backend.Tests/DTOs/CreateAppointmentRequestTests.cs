using backend.DTOs;
using System.ComponentModel.DataAnnotations;

public class CreateAppointmentRequestTests
{
    [Fact]
    public void Validate_EmptyIdsAndDefaultDate_ReturnsValidationErrors()
    {
        var request = new CreateAppointmentRequest
        {
            PatientId = Guid.Empty,
            DentistId = Guid.Empty,
            TreatmentId = Guid.Empty,
            ScheduledAt = default
        };

        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            validationResults,
            validateAllProperties: true);

        Assert.False(isValid);
        Assert.Contains(validationResults, r => r.ErrorMessage == "PatientId is required.");
        Assert.Contains(validationResults, r => r.ErrorMessage == "DentistId is required.");
        Assert.Contains(validationResults, r => r.ErrorMessage == "TreatmentId is required.");
        Assert.Contains(validationResults, r => r.ErrorMessage == "ScheduledAt is required.");
    }
}
