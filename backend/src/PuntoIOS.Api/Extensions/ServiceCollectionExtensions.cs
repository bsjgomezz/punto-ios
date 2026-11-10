using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using PuntoIOS.Api.Authentication;
using PuntoIOS.Api.Seguridad;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Clientes.ActualizarCliente;
using PuntoIOS.Application.Features.Clientes.BuscarClientes;
using PuntoIOS.Application.Features.Clientes.CrearCliente;
using PuntoIOS.Application.Features.Clientes.HistorialCliente;
using PuntoIOS.Application.Features.Clientes.ObtenerCliente;
using PuntoIOS.Application.Features.Clientes.SeguimientoCliente;
using PuntoIOS.Application.Features.Dispositivos.ActualizarDispositivo;
using PuntoIOS.Application.Features.Dispositivos.CrearDispositivo;
using PuntoIOS.Application.Features.Dispositivos.HistorialDispositivo;
using PuntoIOS.Application.Features.Dispositivos.ListarDispositivos;
using PuntoIOS.Application.Features.Dispositivos.ObtenerDispositivo;
using PuntoIOS.Application.Features.Dispositivos.RevelarImei;
using PuntoIOS.Application.Features.Identidad.ActualizarUsuario;
using PuntoIOS.Application.Features.Reparaciones.ActualizarDatosTecnicos;
using PuntoIOS.Application.Features.Reparaciones.CambiarEstado;
using PuntoIOS.Application.Features.Reparaciones.ConsultarOrdenes;
using PuntoIOS.Application.Features.Reparaciones.CrearVersionPresupuesto;
using PuntoIOS.Application.Features.Reparaciones.Fotos;
using PuntoIOS.Application.Features.Reparaciones.ListarVersionesPresupuesto;
using PuntoIOS.Application.Features.Reparaciones.RecibirEquipo;
using PuntoIOS.Application.Features.Reparaciones.VerDesbloqueo;
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
        services.AddDataProtection().SetApplicationName("PuntoIOS");
        services.AddSingleton<ICifradoDeDatos, CifradoConDataProtection>();
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
        services.AddScoped<GenerarCodigoSeguimientoHandler>();
        services.AddScoped<ConsultarCodigoSeguimientoHandler>();
        services.AddScoped<ListarDispositivosHandler>();
        services.AddScoped<ObtenerDispositivoHandler>();
        services.AddScoped<CrearDispositivoHandler>();
        services.AddScoped<ActualizarDispositivoHandler>();
        services.AddScoped<RevelarImeiHandler>();
        services.AddScoped<HistorialDispositivoHandler>();
        services.AddScoped<RecibirEquipoHandler>();
        services.AddScoped<ObtenerOrdenHandler>();
        services.AddScoped<ListarOrdenesHandler>();
        services.AddScoped<ListarTecnicosHandler>();
        services.AddScoped<VerDesbloqueoHandler>();
        services.AddScoped<HistorialOrdenHandler>();
        services.AddScoped<AgregarFotoHandler>();
        services.AddScoped<ListarFotosHandler>();
        services.AddScoped<ObtenerFotoHandler>();
        services.AddScoped<CambiarEstadoOrdenHandler>();
        services.AddScoped<ActualizarDatosTecnicosHandler>();
        services.AddScoped<CrearVersionPresupuestoHandler>();
        services.AddScoped<ListarVersionesPresupuestoHandler>();
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
