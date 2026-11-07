using Microsoft.EntityFrameworkCore;
using PuntoIOS.Domain.Identidad;

namespace PuntoIOS.Infrastructure.Persistence;

/// <summary>Aplica las migraciones y crea el administrador inicial si todavía no existe.</summary>
public sealed class InicializadorBaseDeDatos(PuntoIosDbContext db, TimeProvider reloj)
{
    public async Task EjecutarAsync(string? emailAdministradorInicial, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        var email = Usuario.NormalizarEmail(emailAdministradorInicial ?? string.Empty);
        if (email.Length == 0)
        {
            return;
        }

        if (await db.Usuarios.AnyAsync(u => u.Email == email, ct))
        {
            return;
        }

        var rolAdministrador = await db.Roles.SingleAsync(r => r.Id == RolesDeSistema.AdministradorId, ct);
        db.Usuarios.Add(Usuario.Registrar(email, "Administrador inicial", rolAdministrador, reloj.GetUtcNow()));
        await db.SaveChangesAsync(ct);
    }
}
