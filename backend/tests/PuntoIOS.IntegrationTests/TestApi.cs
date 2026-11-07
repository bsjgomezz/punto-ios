using System.Net.Http.Headers;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using PuntoIOS.Domain.Identidad;
using PuntoIOS.Infrastructure.Persistence;

namespace PuntoIOS.IntegrationTests;

/// <summary>
/// API de prueba: PostgreSQL real (migrado al arrancar, con administrador inicial) y Firebase simulado
/// firmando los tokens con una clave de prueba, para no depender de internet.
/// </summary>
public sealed class TestApi : IDisposable
{
    public const string ProjectId = "proyecto-test";
    public const string EmailAdministrador = "admin@puntoios.test";
    public const string UidAdministrador = "uid-admin";

    private static readonly SymmetricSecurityKey TestKey = new(RandomNumberGenerator.GetBytes(32));

    private readonly WebApplicationFactory<Program> _factory;

    private TestApi(WebApplicationFactory<Program> factory) => _factory = factory;

    public static async Task<TestApi> CrearAsync(PostgresFixture postgres)
    {
        var cadena = await postgres.CrearBaseDeDatosAsync();

        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, config) =>
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Firebase:ProjectId"] = ProjectId,
                    ["ConnectionStrings:Default"] = cadena,
                    ["Database:MigrateOnStartup"] = "true",
                    ["Seed:AdminEmail"] = EmailAdministrador,
                }));
            builder.ConfigureTestServices(services =>
                services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.ConfigurationManager =
                        new StaticConfigurationManager<OpenIdConnectConfiguration>(new OpenIdConnectConfiguration());
                    options.TokenValidationParameters.IssuerSigningKey = TestKey;
                }));
        });

        return new TestApi(factory);
    }

    public HttpClient Cliente(string? bearerToken = null)
    {
        var client = _factory.CreateClient();
        if (bearerToken is not null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        return client;
    }

    /// <summary>Cliente autenticado como el administrador inicial (ya vinculado a su uid de Firebase).</summary>
    public HttpClient ClienteAdministrador() => Cliente(CrearToken(UidAdministrador, EmailAdministrador));

    /// <summary>Registra un usuario directamente en la base y devuelve un token de Firebase válido para él.</summary>
    public async Task<string> RegistrarUsuarioAsync(string email, Guid rolId, bool activo = true)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<PuntoIosDbContext>();
        var rol = await db.Roles.SingleAsync(r => r.Id == rolId);
        var usuario = Usuario.Registrar(email, email, rol, DateTimeOffset.UtcNow);
        if (!activo)
        {
            usuario.Desactivar();
        }

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
        return CrearToken("uid-" + email, email);
    }

    public static string CrearToken(
        string uid,
        string email,
        bool emailVerificado = true,
        string project = ProjectId,
        SecurityKey? key = null,
        TimeSpan? vigencia = null)
    {
        var now = DateTime.UtcNow;
        var expires = now.Add(vigencia ?? TimeSpan.FromHours(1));
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = $"https://securetoken.google.com/{project}",
            Audience = project,
            NotBefore = expires < now ? expires.AddHours(-1) : now.AddMinutes(-1),
            Expires = expires,
            Claims = new Dictionary<string, object>
            {
                ["sub"] = uid,
                ["email"] = email,
                ["email_verified"] = emailVerificado,
                ["name"] = "Usuario de prueba",
            },
            SigningCredentials = new SigningCredentials(key ?? TestKey, SecurityAlgorithms.HmacSha256),
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    public static SymmetricSecurityKey OtraClave() => new(RandomNumberGenerator.GetBytes(32));

    public void Dispose() => _factory.Dispose();
}
