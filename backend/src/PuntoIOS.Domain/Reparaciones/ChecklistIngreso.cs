namespace PuntoIOS.Domain.Reparaciones;

/// <summary>Resultado de revisar un ítem al recibir el equipo. Los ítems que no se revisaron no se guardan.</summary>
public sealed record ItemChecklist(string Codigo, bool Valor);

/// <summary>Checklist de ingreso (todo opcional): qué se revisó al recibir el equipo y cómo estaba.</summary>
public sealed record ChecklistIngreso(IReadOnlyList<ItemChecklist> Items, int? SaludBateria, string? Notas);

public static class ItemsDeChecklist
{
    public const string Enciende = "enciende";
    public const int LargoMaximoNotas = 1000;

    /// <summary>Si el equipo no enciende no se puede determinar la falla completa ni una fecha de entrega.</summary>
    public const string RazonSinFecha = "No enciende y no se puede determinar la falla en su totalidad.";

    /// <summary>Código → (grupo, texto). El orden es el de la pantalla.</summary>
    public static readonly IReadOnlyList<(string Codigo, string Grupo, string Nombre)> Catalogo =
    [
        (Enciende, "Encendido", "Enciende"),
        ("carga", "Encendido", "Carga"),
        ("pantalla", "Encendido", "La pantalla da imagen"),
        ("tactil", "Encendido", "El táctil responde"),
        ("camara_trasera", "Funciones", "Cámara trasera"),
        ("camara_frontal", "Funciones", "Cámara frontal"),
        ("face_id", "Funciones", "Face ID / Touch ID"),
        ("microfono", "Funciones", "Micrófono"),
        ("parlante", "Funciones", "Parlante"),
        ("auricular", "Funciones", "Auricular (llamadas)"),
        ("senal", "Funciones", "Señal"),
        ("wifi", "Funciones", "Wi-Fi"),
        ("botones", "Funciones", "Botones"),
        ("icloud", "Batería y cuenta", "iCloud / Buscar mi iPhone activo"),
        ("sim", "Batería y cuenta", "Trae chip / SIM"),
    ];

    public static bool Existe(string codigo) => Catalogo.Any(i => i.Codigo == codigo);
}
