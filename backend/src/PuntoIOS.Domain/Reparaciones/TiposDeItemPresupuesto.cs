namespace PuntoIOS.Domain.Reparaciones;

/// <summary>Tipos de ítem de un presupuesto: mano de obra o repuesto.</summary>
public static class TiposDeItemPresupuesto
{
    public const string ManoDeObra = "mano_obra";
    public const string Repuesto = "repuesto";

    public static readonly IReadOnlyDictionary<string, string> Catalogo = new Dictionary<string, string>
    {
        [ManoDeObra] = "Mano de obra",
        [Repuesto] = "Repuesto",
    };
}

/// <summary>Un ítem de una versión de presupuesto: mano de obra o repuesto, con su monto.</summary>
public sealed record ItemDePresupuesto(string Descripcion, string Tipo, decimal Monto)
{
    public const int LargoMaximoDescripcion = 200;

    /// <summary>Tope por ítem: con <see cref="VersionDePresupuesto.MaximoItems"/> ítems el total entra en numeric(12,2).</summary>
    public const decimal MontoMaximo = 99_999_999.99m;
}
