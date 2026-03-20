namespace backend.Entities;

// treatment entity instead of enum to allow dynamic treatments without code deploys
public class Treatment
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    /* 
        - assume treatments have fixed durations
        - improve ux to allow dentists set durations per appointment
          with a default for regular durations per treatment
    */
    public int DurationMinutes { get; set; }
}
