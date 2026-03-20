using Azure.Identity;
using backend.Data;
using backend.ExceptionHandlers;
using backend.Repositories;
using backend.Services;
using backend.Storage;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// in memory db 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("DentalClinicDb"));

// repos
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IDentistRepository, DentistRepository>();
builder.Services.AddScoped<ITreatmentRepository, TreatmentRepository>();

// services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IDentistService, DentistService>();
builder.Services.AddScoped<ITreatmentService, TreatmentService>();

// storage - choose implementation based on configuration
var storageProvider = builder.Configuration.GetValue<string>("Storage:Provider");

if (storageProvider == "AzureBlob")
{
    builder.Services.AddAzureClients(clientBuilder =>
    {
        clientBuilder.AddBlobServiceClient(
            builder.Configuration.GetSection("Storage:AzureBlob"));
        clientBuilder.UseCredential(new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned));
    });

    builder.Services.AddScoped<IPhotoStorage, AzureBlobPhotoStorage>();
}
else
{
    builder.Services.AddScoped<IPhotoStorage, LocalPhotoStorage>();
}

// add controllers
builder.Services.AddControllers();

// global error handling
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Dental Clinic API", Version = "v1" });
});

// CORS — loaded from configs
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

// for simplicity, allow any header or method. more restrictions in prod
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// use error handler
app.UseExceptionHandler();

// use swagger in development only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// use cors policy
app.UseCors("AllowFrontend");

// controllers mappers
app.MapControllers();

// seed data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
