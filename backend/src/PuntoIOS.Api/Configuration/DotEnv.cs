namespace PuntoIOS.Api.Configuration;

/// <summary>
/// Carga el archivo <c>.env</c> de la raíz del repositorio en variables de entorno,
/// solo cuando ASPNETCORE_ENVIRONMENT=Development. Nunca pisa variables ya definidas.
/// </summary>
public static class DotEnv
{
    public static void LoadForDevelopment()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (!string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine(directory.FullName, ".env");
            if (File.Exists(path))
            {
                Apply(path);
                return;
            }

            directory = directory.Parent;
        }
    }

    private static void Apply(string path)
    {
        foreach (var rawLine in File.ReadAllLines(path))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            var separator = line.IndexOf('=');
            if (separator <= 0)
            {
                continue;
            }

            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim().Trim('"');
            // Un valor vacío no se aplica: pisaría con "" los valores por defecto de appsettings.
            if (value.Length > 0 && Environment.GetEnvironmentVariable(key) is null)
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}
