using Microsoft.AspNetCore.Authorization;
using PuntoIOS.Api.Authentication;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Clientes.ActualizarCliente;
using PuntoIOS.Application.Features.Clientes.BuscarClientes;
using PuntoIOS.Application.Features.Clientes.CrearCliente;
using PuntoIOS.Application.Features.Clientes.HistorialCliente;
using PuntoIOS.Application.Features.Clientes.ObtenerCliente;
using PuntoIOS.Application.Features.Identidad.ActualizarUsuario;
using PuntoIOS.Application.Features.Identidad.IdentificarUsuario;
using PuntoIOS.Application.Features.Identidad.ListarRoles;
using PuntoIOS.Application.Features.Identidad.ListarUsuarios;
using PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual;
using PuntoIOS.Application.Features.Identidad.RegistrarUsuario;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>Registra los casos de uso de Application y el adaptador del usuario actual.</summary>
    public static IServiceCollection AddCasosDeUso(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
        services.AddScoped<IdentificarUsuarioHandler>();
        services.AddScoped<ObtenerUsuarioActualHandler>();
        services.AddScoped<ListarUsuariosHandler>();
        services.AddScoped<ListarRolesHandler>();
        services.AddScoped<RegistrarUsuarioHandler>();
        services.AddScoped<ActualizarUsuarioHandler>();
        services.AddScoped<CrearClienteHandler>();
        services.AddScoped<BuscarClientesHandler>();
        services.AddScoped<ObtenerClienteHandler>();
        services.AddScoped<ActualizarClienteHandler>();
        services.AddScoped<HistorialClienteHandler>();
        return services;
    }

    /// <summary>
    /// Política por defecto: token válido Y usuario local registrado y activo.
    /// Además una política por cada permiso del catálogo, con el código del permiso como nombre.
    /// </summary>
    public static IServiceCollection AddPoliticasDeAutorizacion(this IServiceCollection services)
    {
        var autorizacion = services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(FirebaseClaims.UsuarioId)
                .Build());

        foreach (var codigo in Permisos.Catalogo.Keys)
        {
            autorizacion.AddPolicy(codigo, policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim(FirebaseClaims.UsuarioId)
                .RequireClaim(FirebaseClaims.Permiso, codigo));
        }

        return services;
    }
}
