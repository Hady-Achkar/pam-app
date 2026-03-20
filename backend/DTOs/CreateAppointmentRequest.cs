namespace backend.DTOs;

public class CreateAppointmentRequest : IValidatableObject
{
    public Guid PatientId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public Guid DentistId { get; set; }
    public Guid TreatmentId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext _)
    {
        if (PatientId == Guid.Empty)
        {
            yield return new ValidationResult(
                "PatientId is required.",
                [nameof(PatientId)]);
        }

        if (DentistId == Guid.Empty)
        {
            yield return new ValidationResult(
                "DentistId is required.",
                [nameof(DentistId)]);
        }

        if (TreatmentId == Guid.Empty)
        {
            yield return new ValidationResult(
                "TreatmentId is required.",
                [nameof(TreatmentId)]);
        }

        if (ScheduledAt == default)
        {
            yield return new ValidationResult(
                "ScheduledAt is required.",
                [nameof(ScheduledAt)]);
        }
        else if (ScheduledAt < DateTime.UtcNow)
        {
            /* 
             * assume appointments can only be scheduled for the future 
             * improve ux to allow past dates for missed appointments, 
               with a policy on scheduling conflicts       
            */
            yield return new ValidationResult(
                "Appointment date should be in the future.",
                [nameof(ScheduledAt)]);
        }
    }
}
