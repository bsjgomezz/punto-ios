using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace PuntoIOS.Api.Health;

/// <summary>Comprueba que PostgreSQL acepta conexiones y responde una consulta simple.</summary>
public sealed class PostgresReadyCheck(IConfiguration configuration) : IHealthCheck
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var connectionString = configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return HealthCheckResult.Unhealthy("Falta ConnectionStrings:Default.");
        }

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(Timeout);

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(timeout.Token);
            await using var command = new NpgsqlCommand("SELECT 1", connection);
            await command.ExecuteScalarAsync(timeout.Token);
            return HealthCheckResult.Healthy();
        }
        catch (Exception exception)
        {
            // La excepción se conserva para logs internos; el cuerpo de la respuesta solo dice Unhealthy.
            return HealthCheckResult.Unhealthy("No se pudo consultar PostgreSQL.", exception);
        }
    }
}
