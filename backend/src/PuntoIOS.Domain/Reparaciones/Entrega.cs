namespace PuntoIOS.Domain.Reparaciones;

/// <summary>
/// Opciones de garantía al entregar el equipo. La duración elegida y la fecha de vencimiento quedan grabadas
/// directo en la orden (snapshot): un cambio futuro en este catálogo nunca altera una entrega ya hecha.
/// </summary>
public static class OpcionesDeGarantia
{
    public const string SinGarantia = "sin_garantia";
    public const string Dias15 = "15_dias";
    public const string Dias30 = "30_dias";
    public const string Dias45 = "45_dias";
    public const string Dias60 = "60_dias";

    /// <summary>Duración en días de cada opción; null para "sin garantía".</summary>
    public static readonly IReadOnlyDictionary<string, int?> Duraciones = new Dictionary<string, int?>
    {
        [SinGarantia] = null,
        [Dias15] = 15,
        [Dias30] = 30,
        [Dias45] = 45,
        [Dias60] = 60,
    };

    public static readonly IReadOnlyDictionary<string, string> Catalogo = new Dictionary<string, string>
    {
        [SinGarantia] = "Sin garantía",
        [Dias15] = "15 días",
        [Dias30] = "30 días",
        [Dias45] = "45 días",
        [Dias60] = "60 días",
    };
}

/// <summary>Métodos de pago aceptados (SDD §8). La Fase 11 (finanzas) los va a reusar para todo el resto del sistema.</summary>
public static class MetodosDePago
{
    public const string Efectivo = "efectivo";
    public const string TransferenciaBancaria = "transferencia_bancaria";
    public const string TarjetaDebito = "tarjeta_debito";
    public const string TarjetaCredito = "tarjeta_credito";

    public static readonly IReadOnlyDictionary<string, string> Catalogo = new Dictionary<string, string>
    {
        [Efectivo] = "Efectivo",
        [TransferenciaBancaria] = "Transferencia bancaria",
        [TarjetaDebito] = "Tarjeta de débito",
        [TarjetaCredito] = "Tarjeta de crédito",
    };
}
