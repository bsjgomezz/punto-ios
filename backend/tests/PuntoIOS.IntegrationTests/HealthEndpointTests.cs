using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace PuntoIOS.IntegrationTests;

public class HealthEndpointTests(PostgresFixture postgres)
    : IClassFixture<PostgresFixture>
{
    private static HttpClient CreateClient(string? connectionString)
    {
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                if (connectionString is not null)
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:Default"] = connectionString,
                    });
                }
            });
        });

        return factory.CreateClient();
    }

    [Fact]
    public async Task Health_responde_200_sin_necesitar_base_de_datos()
    {
        using var client = CreateClient(connectionString: null);

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Ready_responde_200_con_postgres_real()
    {
        using var client = CreateClient(postgres.ConnectionString);

        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Ready_responde_503_si_postgres_no_esta_disponible()
    {
        using var client = CreateClient("Host=127.0.0.1;Port=1;Database=x;Username=x;Password=x;Timeout=2");

        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task Ready_responde_503_si_falta_la_cadena_de_conexion()
    {
        using var client = CreateClient(connectionString: null);

        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }
}
