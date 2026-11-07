using Npgsql;
using Testcontainers.PostgreSql;

namespace PuntoIOS.IntegrationTests;

/// <summary>Levanta un PostgreSQL real en Docker para las pruebas de integración.</summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:16").Build();

    public string ConnectionString => _container.GetConnectionString();

    /// <summary>Crea una base vacía y devuelve su cadena de conexión: cada prueba trabaja aislada de las demás.</summary>
    public async Task<string> CrearBaseDeDatosAsync()
    {
        var nombre = "test_" + Guid.NewGuid().ToString("N");

        await using (var conexion = new NpgsqlConnection(ConnectionString))
        {
            await conexion.OpenAsync();
            await using var comando = new NpgsqlCommand($"CREATE DATABASE \"{nombre}\"", conexion);
            await comando.ExecuteNonQueryAsync();
        }

        return new NpgsqlConnectionStringBuilder(ConnectionString) { Database = nombre, Pooling = false }.ConnectionString;
    }

    public Task InitializeAsync() => _container.StartAsync();

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();
}
