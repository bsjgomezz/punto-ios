using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual;

namespace PuntoIOS.IntegrationTests;

/// <summary>
/// Verifica la validación del Firebase ID Token. Se firma con una clave de prueba y se
/// reemplaza la descarga de claves de Google, así la prueba no depende de internet.
/// </summary>
public class MeEndpointTests
{
    private const string ProjectId = "proyecto-test";
    private static readonly SymmetricSecurityKey TestKey = new(RandomNumberGenerator.GetBytes(32));

    private static HttpClient CreateClient(string? bearerToken = null)
    {
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, config) =>
                config.AddInMemoryCollection(new Dictionary<string, string?> { ["Firebase:ProjectId"] = ProjectId }));
            builder.ConfigureTestServices(services =>
                services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.ConfigurationManager =
                        new StaticConfigurationManager<OpenIdConnectConfiguration>(new OpenIdConnectConfiguration());
                    options.TokenValidationParameters.IssuerSigningKey = TestKey;
                }));
        });

        var client = factory.CreateClient();
        if (bearerToken is not null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        return client;
    }

    private static string CreateToken(
        string project = ProjectId,
        SecurityKey? key = null,
        TimeSpan? lifetime = null)
    {
        var now = DateTime.UtcNow;
        var expires = now.Add(lifetime ?? TimeSpan.FromHours(1));
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = $"https://securetoken.google.com/{project}",
            Audience = project,
            NotBefore = expires < now ? expires.AddHours(-1) : now.AddMinutes(-1),
            Expires = expires,
            Claims = new Dictionary<string, object>
            {
                ["sub"] = "uid-123",
                ["email"] = "dueno@puntoios.com",
                ["email_verified"] = true,
                ["name"] = "Dueño",
            },
            SigningCredentials = new SigningCredentials(key ?? TestKey, SecurityAlgorithms.HmacSha256),
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    [Fact]
    public async Task Sin_token_responde_401()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Con_un_token_basura_responde_401()
    {
        using var client = CreateClient("esto-no-es-un-jwt");

        var response = await client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Con_firma_de_otra_clave_responde_401()
    {
        var otraClave = new SymmetricSecurityKey(RandomNumberGenerator.GetBytes(32));
        using var client = CreateClient(CreateToken(key: otraClave));

        var response = await client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Con_otro_proyecto_de_firebase_responde_401()
    {
        using var client = CreateClient(CreateToken(project: "otro-proyecto"));

        var response = await client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Con_token_vencido_responde_401()
    {
        using var client = CreateClient(CreateToken(lifetime: TimeSpan.FromHours(-1)));

        var response = await client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Con_token_valido_devuelve_la_identidad()
    {
        using var client = CreateClient(CreateToken());

        var response = await client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var usuario = await response.Content.ReadFromJsonAsync<UsuarioActualDto>();
        Assert.Equal(new UsuarioActualDto("uid-123", "dueno@puntoios.com", true, "Dueño"), usuario);
    }
}
