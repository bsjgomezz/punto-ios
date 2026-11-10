namespace PuntoIOS.Domain.Reparaciones;

/// <summary>
/// Una versión del presupuesto de una orden. Nunca se edita: "editar" el presupuesto es crear una versión nueva.
/// La vigente es la de número más alto. El total es la suma de los ítems.
/// </summary>
public sealed class VersionDePresupuesto
{
    public const int MaximoItems = 30;

    private VersionDePresupuesto()
    {
    }

    public Guid Id { get; private set; }

    public Guid LocalId { get; private set; }

    public Guid OrdenId { get; private set; }

    /// <summary>1, 2, 3... por orden. La vigente es la de número más alto.</summary>
    public int Numero { get; private set; }

    public List<ItemDePresupuesto> Items { get; private set; } = [];

    public decimal Total { get; private set; }

    public Guid? CreadoPorUsuarioId { get; private set; }

    public DateTimeOffset CreadoEn { get; private set; }

    /// <summary>
    /// Crea la versión número <paramref name="numero"/> con estos ítems. Valida la cantidad y cada ítem
    /// (descripción obligatoria y sin pasarse de largo, tipo del catálogo, monto mayor a cero con hasta 2 decimales).
    /// Las descripciones se guardan sin espacios de más.
    /// </summary>
    public static VersionDePresupuesto Crear(
        Guid localId, Guid ordenId, int numero, List<ItemDePresupuesto> items, Guid? creadoPorUsuarioId, DateTimeOffset ahora)
    {
        if (numero < 1)
        {
            throw new ArgumentException("El número de versión empieza en 1.", nameof(numero));
        }

        if (items.Count == 0)
        {
            throw new ArgumentException("El presupuesto necesita al menos un ítem.", nameof(items));
        }

        if (items.Count > MaximoItems)
        {
            throw new ArgumentException($"El presupuesto admite hasta {MaximoItems} ítems.", nameof(items));
        }

        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.Descripcion))
            {
                throw new ArgumentException("Cada ítem necesita una descripción.", nameof(items));
            }

            if (item.Descripcion.Trim().Length > ItemDePresupuesto.LargoMaximoDescripcion)
            {
                throw new ArgumentException(
                    $"La descripción de un ítem admite hasta {ItemDePresupuesto.LargoMaximoDescripcion} caracteres.", nameof(items));
            }

            if (item.Tipo is null || !TiposDeItemPresupuesto.Catalogo.ContainsKey(item.Tipo))
            {
                throw new ArgumentException("El tipo de ítem no existe.", nameof(items));
            }

            if (item.Monto <= 0)
            {
                throw new ArgumentException("El monto de cada ítem tiene que ser mayor a cero.", nameof(items));
            }

            if (item.Monto > ItemDePresupuesto.MontoMaximo || decimal.Round(item.Monto, 2) != item.Monto)
            {
                throw new ArgumentException(
                    "El monto de cada ítem admite hasta 2 decimales y no puede superar 99.999.999,99.", nameof(items));
            }
        }

        var normalizados = items.Select(i => i with { Descripcion = i.Descripcion.Trim() }).ToList();
        return new VersionDePresupuesto
        {
            Id = Guid.CreateVersion7(),
            LocalId = localId,
            OrdenId = ordenId,
            Numero = numero,
            Items = normalizados,
            Total = normalizados.Sum(i => i.Monto),
            CreadoPorUsuarioId = creadoPorUsuarioId,
            CreadoEn = ahora,
        };
    }
}
