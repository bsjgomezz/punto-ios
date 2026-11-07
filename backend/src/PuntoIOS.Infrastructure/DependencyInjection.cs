using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Dispositivos.Puertos;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Application.Features.Reparaciones.Puertos;
using PuntoIOS.Infrastructure.Persistence;

namespace PuntoIOS.Infrastructure;

public static class DependencyInjection
{
    /// <summary>Registra la persistencia (EF Core + PostgreSQL) y los adaptadores de los puertos de Application.</summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        Func<IServiceProvider, string?> connectionString)
    {
        // La cadena se lee de forma diferida (al crear el contexto) para respetar la configuración final,
        // incluida la que agregan las pruebas de integración.
        services.AddDbContext<PuntoIosDbContext>((sp, options) =>
        {
            // Sin cadena de conexión el contexto se crea igual; las consultas fallan y /health/ready lo informa.
            var cadena = connectionString(sp);
            options.UseNpgsql(string.IsNullOrWhiteSpace(cadena)
                ? "Host=localhost;Database=puntoios_sin_configurar"
                : cadena);
        });
        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<IRolRepositorio, RolRepositorio>();
        services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
        services.AddScoped<IDispositivoRepositorio, DispositivoRepositorio>();
        services.AddScoped<IOrdenReparacionRepositorio, OrdenReparacionRepositorio>();
        services.AddScoped<IAuditoriaRepositorio, AuditoriaRepositorio>();
        services.AddScoped<IDirectorioDeUsuarios, DirectorioDeUsuarios>();
        services.AddSingleton<ILocalActual, LocalUnico>();
        services.AddScoped<InicializadorBaseDeDatos>();
        return services;
    }
}
