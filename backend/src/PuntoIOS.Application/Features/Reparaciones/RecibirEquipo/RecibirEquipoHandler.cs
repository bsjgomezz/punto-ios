using PuntoIOS.Application.Common.Multilocal;
using PuntoIOS.Application.Common.Results;
using PuntoIOS.Application.Common.Security;
using PuntoIOS.Application.Features.Auditoria.Puertos;
using PuntoIOS.Application.Features.Clientes.CrearCliente;
using PuntoIOS.Application.Features.Clientes.Puertos;
using PuntoIOS.Application.Features.Dispositivos;
using PuntoIOS.Application.Features.Dispositivos.Puertos;
using PuntoIOS.Application.Features.Identidad.Puertos;
using PuntoIOS.Application.Features.Reparaciones.Puertos;
using PuntoIOS.Domain.Auditoria;
using PuntoIOS.Domain.Clientes;
using PuntoIOS.Domain.Identidad;
using PuntoIOS.Domain.Reparaciones;

namespace PuntoIOS.Application.Features.Reparaciones.RecibirEquipo;

/// <summary>
/// Datos de la recepción. El equipo es uno ya registrado del cliente (<c>DispositivoId</c>) o uno nuevo
/// (<c>EquipoNuevo</c>), nunca los dos.
/// </summary>
public sealed record RecibirEquipoCommand(
    Guid ClienteId,
    Guid? DispositivoId,
    EquipoNuevo? EquipoNuevo,
    string? ProblemaInformado,
    IReadOnlyList<string>? TiposReparacion,
    string? OtroTipoDescripcion,
    Guid? TecnicoId,
    DateOnly? FechaEstimadaEntrega,
    string? DesbloqueoTipo,
    string? DesbloqueoValor,
    ChecklistIngreso? Checklist = null);

/// <summary>Recibe un equipo en el taller y crea la orden de reparación (y el equipo, si es nuevo) en una sola operación.</summary>
public sealed class RecibirEquipoHandler(
    IOrdenReparacionRepositorio ordenes,
    IClienteRepositorio clientes,
    IDispositivoRepositorio dispositivos,
    IAuditoriaRepositorio auditoria,
    IDirectorioDeUsuarios directorio,
    ICifradoDeDatos cifrado,
    ILocalActual local,
    ICurrentUserAccessor usuarioActual,
    TimeProvider reloj)
{
    public async Task<Resultado<OrdenReparacionDto>> HandleAsync(RecibirEquipoCommand comando, CancellationToken ct)
    {
        var ahora = reloj.GetUtcNow();

        var error = ValidarDatos(comando, ahora, out var desbloqueo);
        if (error is not null)
        {
            return Resultado<OrdenReparacionDto>.Falla(error);
        }

        var cliente = await clientes.ObtenerPorIdAsync(comando.ClienteId, ct);
        if (cliente is null)
        {
            return Resultado<OrdenReparacionDto>.Falla(TipoError.NoEncontrado, "cliente_inexistente", "El cliente no existe.");
        }

        if (comando.TecnicoId is { } tecnicoId
            && (await directorio.ListarActivosConPermisoAsync(Permisos.ReparacionesReparar, ct)).All(u => u.Id != tecnicoId))
        {
            return Falla("tecnico_invalido", "El técnico elegido no existe, está inactivo o no puede tomar reparaciones.");
        }

        var usuarioId = usuarioActual.GetCurrentUser()?.UsuarioId;

        Dispositivo equipo;
        if (comando.DispositivoId is { } dispositivoId)
        {
            var existente = await dispositivos.ObtenerPorIdAsync(dispositivoId, ct);
            if (existente is null || existente.ClienteId != cliente.Id)
            {
                return Falla("equipo_invalido", "El equipo elegido no pertenece a este cliente.");
            }

            equipo = existente;
        }
        else
        {
            var nuevo = comando.EquipoNuevo!;
            var errorEquipo = DispositivoValidacion.Validar(nuevo.Marca, nuevo.Modelo, nuevo.Almacenamiento, nuevo.Color, nuevo.ImeiSerie, null);
            if (errorEquipo is not null)
            {
                return Resultado<OrdenReparacionDto>.Falla(errorEquipo);
            }

            var imei = Dispositivo.NormalizarImei(nuevo.ImeiSerie);
            if (imei is not null && await dispositivos.ObtenerPorImeiAsync(imei, ct) is { } conEseImei)
            {
                return Resultado<OrdenReparacionDto>.Falla(
                    DispositivoValidacion.ImeiDuplicado(await clientes.ObtenerPorIdAsync(conEseImei.ClienteId, ct)));
            }

            equipo = Dispositivo.Registrar(
                local.LocalId, cliente.Id, nuevo.Marca!, nuevo.Modelo!, nuevo.Almacenamiento, nuevo.Color, nuevo.ImeiSerie, null, usuarioId, ahora);
            dispositivos.Agregar(equipo);
            auditoria.Agregar(RegistroAuditoria.Registrar(
                local.LocalId, EntidadesAuditadas.Dispositivo, equipo.Id, AccionesAuditadas.Creado, usuarioId, ahora));
        }

        var orden = OrdenReparacion.Recibir(
            local.LocalId,
            cliente.Id,
            equipo.Id,
            comando.ProblemaInformado!,
            comando.TiposReparacion!,
            comando.OtroTipoDescripcion,
            comando.TecnicoId,
            comando.FechaEstimadaEntrega,
            desbloqueo is null ? null : comando.DesbloqueoTipo,
            desbloqueo is null ? null : cifrado.Cifrar(desbloqueo),
            usuarioId,
            ahora,
            comando.Checklist);

        ordenes.Agregar(orden);
        auditoria.Agregar(RegistroAuditoria.Registrar(
            local.LocalId, EntidadesAuditadas.Reparacion, orden.Id, AccionesAuditadas.Creado, usuarioId, ahora));

        // Orden, equipo nuevo y auditoría comparten la unidad de trabajo: se guardan juntos o nada.
        await ordenes.GuardarCambiosAsync(ct);

        var armador = new ArmadorDeOrdenes(clientes, dispositivos, directorio);
        return Resultado<OrdenReparacionDto>.Ok((await armador.CompletasAsync([orden], ct))[0]);
    }

    private static Error? ValidarDatos(RecibirEquipoCommand c, DateTimeOffset ahora, out string? desbloqueo)
    {
        desbloqueo = null;

        if ((c.DispositivoId is null) == (c.EquipoNuevo is null))
        {
            return Invalido("equipo_obligatorio", "Elegí uno de los equipos del cliente o cargá uno nuevo.");
        }

        if (string.IsNullOrWhiteSpace(c.ProblemaInformado))
        {
            return Invalido("problema_obligatorio", "Contá qué problema informa el cliente.");
        }

        if (c.ProblemaInformado.Trim().Length > OrdenReparacion.LargoMaximoProblema)
        {
            return Invalido("texto_demasiado_largo", $"El problema admite hasta {OrdenReparacion.LargoMaximoProblema} caracteres.");
        }

        var tipos = c.TiposReparacion ?? [];
        if (tipos.Count == 0)
        {
            return Invalido("tipo_obligatorio", "Marcá al menos un tipo de reparación.");
        }

        if (tipos.Any(t => !TiposDeReparacion.Catalogo.ContainsKey(t)))
        {
            return Invalido("tipo_invalido", "Hay un tipo de reparación que no existe.");
        }

        if (tipos.Contains(TiposDeReparacion.Otro))
        {
            if (string.IsNullOrWhiteSpace(c.OtroTipoDescripcion))
            {
                return Invalido("otro_tipo_obligatorio", "Describí el otro tipo de reparación.");
            }

            if (c.OtroTipoDescripcion.Trim().Length > OrdenReparacion.LargoMaximoOtroTipo)
            {
                return Invalido("texto_demasiado_largo", $"La descripción admite hasta {OrdenReparacion.LargoMaximoOtroTipo} caracteres.");
            }
        }

        if (c.Checklist is { } checklist)
        {
            if (checklist.Items.Any(i => !ItemsDeChecklist.Existe(i.Codigo))
                || checklist.Items.Select(i => i.Codigo).Distinct().Count() != checklist.Items.Count)
            {
                return Invalido("checklist_invalido", "El checklist tiene ítems que no existen o repetidos.");
            }

            if (checklist.SaludBateria is < 0 or > 100)
            {
                return Invalido("bateria_invalida", "La salud de la batería va de 0 a 100 %.");
            }

            if ((checklist.Notas?.Trim().Length ?? 0) > ItemsDeChecklist.LargoMaximoNotas)
            {
                return Invalido("texto_demasiado_largo", $"Las notas del checklist admiten hasta {ItemsDeChecklist.LargoMaximoNotas} caracteres.");
            }

            if (c.FechaEstimadaEntrega is not null
                && checklist.Items.Any(i => i.Codigo == ItemsDeChecklist.Enciende && !i.Valor))
            {
                return Invalido("fecha_no_determinable", $"Si el equipo no enciende no se puede estimar la entrega. {ItemsDeChecklist.RazonSinFecha}");
            }
        }

        // Margen de un día por la diferencia horaria entre el servidor (UTC) y el local.
        if (c.FechaEstimadaEntrega is { } fecha && fecha < DateOnly.FromDateTime(ahora.UtcDateTime.AddDays(-1)))
        {
            return Invalido("fecha_invalida", "La fecha estimada de entrega no puede ser anterior a hoy.");
        }

        var hayTipo = !string.IsNullOrWhiteSpace(c.DesbloqueoTipo);
        var hayValor = !string.IsNullOrWhiteSpace(c.DesbloqueoValor);
        if (hayTipo || hayValor)
        {
            if (!hayTipo || !TiposDeDesbloqueo.Todos.Contains(c.DesbloqueoTipo!))
            {
                return Invalido("desbloqueo_invalido", "Elegí si el desbloqueo es PIN, contraseña o patrón.");
            }

            desbloqueo = TiposDeDesbloqueo.Normalizar(c.DesbloqueoTipo!, c.DesbloqueoValor);
            if (desbloqueo is null)
            {
                return Invalido(
                    "desbloqueo_invalido",
                    c.DesbloqueoTipo switch
                    {
                        TiposDeDesbloqueo.Pin => "El PIN debe tener entre 4 y 16 números.",
                        TiposDeDesbloqueo.Patron => "El patrón debe unir al menos 4 puntos distintos.",
                        _ => "Escribí la contraseña (hasta 100 caracteres).",
                    });
            }
        }

        return null;
    }

    private static Resultado<OrdenReparacionDto> Falla(string codigo, string mensaje) =>
        Resultado<OrdenReparacionDto>.Falla(TipoError.Validacion, codigo, mensaje);

    private static Error Invalido(string codigo, string mensaje) => new(TipoError.Validacion, codigo, mensaje);
}
