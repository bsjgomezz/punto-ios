using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using PuntoIOS.Api.Authentication;
using PuntoIOS.Api.Configuration;
using PuntoIOS.Api.Health;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual;

DotEnv.LoadForDevelopment();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();

var allowedOrigins = (builder.Configuration["Cors:AllowedOrigins"] ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()));

// Autenticación: Firebase ID Token (JWT). Las claves públicas se descargan de Google.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// Se configura de forma diferida para leer Firebase:ProjectId de la configuración final.
builder.Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IConfiguration>((options, configuration) =>
    {
        var projectId = configuration["Firebase:ProjectId"];
        options.MapInboundClaims = false; // se conservan los nombres originales: sub, email, name...

        if (string.IsNullOrWhiteSpace(projectId))
        {
            return; // sin proyecto configurado ningún token es válido (siempre 401)
        }

        var issuer = $"https://securetoken.google.com/{projectId}";
        options.Authority = issuer;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true,
            RequireSignedTokens = true,
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
builder.Services.AddScoped<ObtenerUsuarioActualHandler>();

builder.Services.AddHealthChecks()
    .AddCheck<PostgresReadyCheck>("postgres", tags: ["ready"]);

var app = builder.Build();

if (string.IsNullOrWhiteSpace(app.Configuration["Firebase:ProjectId"]))
{
    app.Logger.LogWarning("Falta Firebase:ProjectId: todas las solicitudes autenticadas serán rechazadas.");
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Configuration.GetValue<bool>("Https:Redirect"))
{
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseAuthentication();
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
