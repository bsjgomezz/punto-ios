using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PuntoIOS.Infrastructure.Persistence;

/// <summary>Solo para <c>dotnet ef</c>: generar migraciones no abre conexión, alcanza con una cadena de ejemplo.</summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PuntoIosDbContext>
{
    public PuntoIosDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Database=puntoios;Username=puntoios";

        var options = new DbContextOptionsBuilder<PuntoIosDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new PuntoIosDbContext(options, new LocalUnico());
    }
}
