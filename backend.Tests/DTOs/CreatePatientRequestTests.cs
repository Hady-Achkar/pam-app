using backend.DTOs;
using System.ComponentModel.DataAnnotations;

public class CreatePatientRequestTests
{
    [Fact]
    public void Validate_WhitespaceOnlyFullName_ReturnsValidationError()
    {
        var request = new CreatePatientRequest
        {
            FullName = "   ",
            Address = "123 Main St"
        };

        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            validationResults,
            validateAllProperties: true);

        Assert.Contains(validationResults, r =>
            r.ErrorMessage == "Full name is required.");
    }

    [Fact]
    public void Validate_WhitespaceOnlyAddress_ReturnsValidationError()
    {
        var request = new CreatePatientRequest
        {
            FullName = "Jane Doe",
            Address = "   "
        };

        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            validationResults,
            validateAllProperties: true);

        Assert.Contains(validationResults, r =>
            r.ErrorMessage == "Address is required.");
    }
}
