namespace backend.Entities;

public class Appointment
{
    public Guid Id { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid DentistId { get; set; }
    public Dentist Dentist { get; set; } = null!;

    public Guid TreatmentId { get; set; }
    public Treatment Treatment { get; set; } = null!;
}