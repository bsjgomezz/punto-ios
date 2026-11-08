using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using PuntoIOS.Api.Authentication;
using PuntoIOS.Api.Configuration;
using PuntoIOS.Api.Extensions;
using PuntoIOS.Api.Health;
using PuntoIOS.Infrastructure;
using PuntoIOS.Infrastructure.Persistence;

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
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = UsuarioLocalClaims.AsociarAsync,
            OnAuthenticationFailed = context =>
            {
                // Se registra el motivo (sin el token) para poder diagnosticar un 401.
                context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("PuntoIOS.Autenticacion")
                    .LogWarning(
                        "Autenticación rechazada ({Tipo}): {Motivo}",
                        context.Exception.GetType().Name,
                        context.Exception.Message);
                return Task.CompletedTask;
            },
        };

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

builder.Services.AddPoliticasDeAutorizacion();
builder.Services.AddInfrastructure(
    sp => sp.GetRequiredService<IConfiguration>().GetConnectionString("Default"),
    sp => sp.GetRequiredService<IConfiguration>()["Archivos:Carpeta"]);
builder.Services.AddCasosDeUso();

builder.Services.AddHealthChecks()
    .AddCheck<PostgresReadyCheck>("postgres", tags: ["ready"]);

var app = builder.Build();

if (string.IsNullOrWhiteSpace(app.Configuration["Firebase:ProjectId"]))
{
    app.Logger.LogWarning("Falta Firebase:ProjectId: todas las solicitudes autenticadas serán rechazadas.");
}

// Aplica migraciones y crea el administrador inicial (Seed:AdminEmail). Se activa por configuración.
if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<InicializadorBaseDeDatos>()
        .EjecutarAsync(app.Configuration["Seed:AdminEmail"]);
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
