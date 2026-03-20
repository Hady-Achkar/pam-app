namespace backend.DTOs;

public class CreatePatientRequest : IValidatableObject
{
    public required string FullName { get; set; }

    public required string Address { get; set; }

    public IFormFile? Photo { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext _)
    {
        if (string.IsNullOrWhiteSpace(FullName))
        {
            yield return new ValidationResult(
                "Full name is required.",
                [nameof(FullName)]);
        }
        else
        {
            var trimmedName = FullName.Trim();

            if (trimmedName.Length < 2)
            {
                yield return new ValidationResult(
                    "Full name should be at least 2 characters.",
                    [nameof(FullName)]);
            }
            else if (trimmedName.Length > 100)
            {
                yield return new ValidationResult(
                    "Full name should not exceed 100 characters.",
                    [nameof(FullName)]);
            }
        }

        if (string.IsNullOrWhiteSpace(Address))
        {
            yield return new ValidationResult(
                "Address is required.",
                [nameof(Address)]);
        }
        else
        {
            var trimmedAddress = Address.Trim();

            if (trimmedAddress.Length > 250)
            {
                yield return new ValidationResult(
                    "Address should not exceed 250 characters.",
                    [nameof(Address)]);
            }
        }
    }
}
