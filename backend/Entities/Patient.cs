namespace backend.Entities;

public class Patient
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string Address { get; set; }
    // assume patients photos are optional
    public string? PhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Appointment> Appointments { get; set; } = [];
}
