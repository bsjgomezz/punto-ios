namespace PuntoIOS.Domain.Locales;

/// <summary>Local (sucursal) del negocio. En la etapa 1 existe uno solo, pero toda entidad operativa ya guarda su LocalId.</summary>
public sealed class Local
{
    private Local()
    {
    }

    public Local(Guid id, string nombre)
    {
        Id = id;
        Nombre = nombre;
    }

    public Guid Id { get; private set; }

    public string Nombre { get; private set; } = string.Empty;
}

public static class LocalesDeSistema
{
    /// <summary>Único local de la etapa 1 (se siembra en la base de datos).</summary>
    public static readonly Guid PrincipalId = new("0195a000-0000-7000-8000-0000000000a1");

    public const string PrincipalNombre = "Punto iOS";
}
