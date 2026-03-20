namespace backend.Entities;

// simple dentist entity, can be expanded later.
public class Dentist
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}