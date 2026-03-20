namespace backend.Data;

using backend.Entities;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Dentist> Dentists { get; set; }
    public DbSet<Treatment> Treatments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigurePatient(modelBuilder);
        ConfigureAppointment(modelBuilder);
        ConfigureDentist(modelBuilder);
        ConfigureTreatment(modelBuilder);
        SeedData(modelBuilder);
    }

    private static void ConfigurePatient(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(p => p.FullName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Address).IsRequired().HasMaxLength(250);
        });
    }

    private static void ConfigureAppointment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasOne(a => a.Patient)
                  .WithMany(p => p.Appointments)
                  .HasForeignKey(a => a.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Dentist)
                  .WithMany()
                  .HasForeignKey(a => a.DentistId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Treatment)
                  .WithMany()
                  .HasForeignKey(a => a.TreatmentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureDentist(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dentist>(entity =>
        {
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
        });
    }

    private static void ConfigureTreatment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Treatment>(entity =>
        {
            entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
        });
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // given the scope of the project, it's fine to seed hardcoded dentists and treatments
        modelBuilder.Entity<Dentist>().HasData(
            new Dentist { Id = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000001"), Name = "Dr. John Doe" },
            new Dentist { Id = Guid.Parse("a1b2c3d4-0002-0000-0000-000000000002"), Name = "Dr. Jane Doe" },
            new Dentist { Id = Guid.Parse("a1b2c3d4-0003-0000-0000-000000000003"), Name = "Dr. Richard Roe" }
        );

        modelBuilder.Entity<Treatment>().HasData(
            new Treatment { Id = Guid.Parse("b1b2c3d4-0001-0000-0000-000000000001"), Name = "Cleaning", DurationMinutes = 30 },
            new Treatment { Id = Guid.Parse("b1b2c3d4-0002-0000-0000-000000000002"), Name = "Filling", DurationMinutes = 45 },
            new Treatment { Id = Guid.Parse("b1b2c3d4-0003-0000-0000-000000000003"), Name = "Extraction", DurationMinutes = 60 },
            new Treatment { Id = Guid.Parse("b1b2c3d4-0004-0000-0000-000000000004"), Name = "Root Canal", DurationMinutes = 90 }
        );
    }
}
