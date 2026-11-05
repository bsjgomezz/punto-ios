using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using PuntoIOS.Api.Configuration;
using PuntoIOS.Api.Health;

DotEnv.LoadForDevelopment();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var allowedOrigins = (builder.Configuration["Cors:AllowedOrigins"] ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.AddHealthChecks()
    .AddCheck<PostgresReadyCheck>("postgres", tags: ["ready"]);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Configuration.GetValue<bool>("Https:Redirect"))
{
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseAuthorization();

// /health: el proceso está vivo (no consulta dependencias).
app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false });

// /health/ready: además verifica PostgreSQL.
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready"),
});

app.MapControllers();

app.Run();

// Necesario para las pruebas de integración (WebApplicationFactory<Program>).
public partial class Program;
