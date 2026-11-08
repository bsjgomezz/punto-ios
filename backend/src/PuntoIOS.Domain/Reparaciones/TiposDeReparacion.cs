namespace PuntoIOS.Domain.Reparaciones;

/// <summary>Tipos de reparación que se pueden marcar al recibir un equipo (uno o varios).</summary>
public static class TiposDeReparacion
{
    public const string Diagnostico = "diagnostico";
    public const string Pantalla = "pantalla";
    public const string Bateria = "bateria";
    public const string ConectorDeCarga = "conector_carga";
    public const string Camara = "camara";
    public const string Audio = "audio";
    public const string Software = "software";
    public const string Microsoldadura = "microsoldadura";
    public const string FaceId = "face_id";
    public const string CambioDeIc = "cambio_ic";
    public const string Placa = "placa";
    public const string RecuperacionDeDatos = "recuperacion_datos";

    /// <summary>Requiere una descripción.</summary>
    public const string Otro = "otro";

    public static readonly IReadOnlyDictionary<string, string> Catalogo = new Dictionary<string, string>
    {
        [Diagnostico] = "Diagnóstico",
        [Pantalla] = "Pantalla",
        [Bateria] = "Batería",
        [ConectorDeCarga] = "Conector de carga",
        [Camara] = "Cámara",
        [Audio] = "Audio",
        [Software] = "Software",
        [Microsoldadura] = "Microsoldadura",
        [FaceId] = "Face ID",
        [CambioDeIc] = "Cambio de IC",
        [Placa] = "Placa",
        [RecuperacionDeDatos] = "Recuperación de datos",
        [Otro] = "Otro",
    };
}

/// <summary>Estados de una orden. Toda orden nace "recibido"; las transiciones permitidas están en <see cref="MaquinaDeEstados"/>.</summary>
public static class EstadosDeReparacion
{
    public const string Recibido = "recibido";
    public const string EnPruebas = "en_pruebas";
    public const string EnDiagnostico = "en_diagnostico";
    public const string EsperandoPresupuesto = "esperando_presupuesto";
    public const string PresupuestoEnviado = "presupuesto_enviado";
    public const string EsperandoAprobacion = "esperando_aprobacion";
    public const string PresupuestoRechazado = "presupuesto_rechazado";
    public const string Aprobado = "aprobado";
    public const string EnReparacion = "en_reparacion";
    public const string EsperandoRepuesto = "esperando_repuesto";
    public const string ReparacionCompletada = "reparacion_completada";
    public const string ListoParaRetirar = "listo_para_retirar";

    /// <summary>Terminal. Al entregar el equipo se borra el código de desbloqueo.</summary>
    public const string Entregado = "entregado";

    /// <summary>Terminal.</summary>
    public const string Cancelado = "cancelado";

    public const string Irreparable = "irreparable";

    public static readonly IReadOnlyDictionary<string, string> Catalogo = new Dictionary<string, string>
    {
        [Recibido] = "Recibido",
        [EnPruebas] = "En pruebas",
        [EnDiagnostico] = "En diagnóstico",
        [EsperandoPresupuesto] = "Esperando presupuesto",
        [PresupuestoEnviado] = "Presupuesto enviado",
        [EsperandoAprobacion] = "Esperando aprobación",
        [PresupuestoRechazado] = "Presupuesto rechazado",
        [Aprobado] = "Aprobado",
        [EnReparacion] = "En reparación",
        [EsperandoRepuesto] = "Esperando repuesto",
        [ReparacionCompletada] = "Reparación completada",
        [ListoParaRetirar] = "Listo para retirar",
        [Entregado] = "Entregado",
        [Cancelado] = "Cancelado",
        [Irreparable] = "Irreparable",
    };
}

/// <summary>Cómo se desbloquea el equipo.</summary>
public static class TiposDeDesbloqueo
{
    public const string Pin = "pin";
    public const string Contrasena = "contrasena";

    /// <summary>Puntos de una grilla de 3×3 numerados del 1 al 9, en orden, separados por guiones: "1-5-9-6".</summary>
    public const string Patron = "patron";

    public static readonly IReadOnlySet<string> Todos = new HashSet<string> { Pin, Contrasena, Patron };

    /// <summary>Valida el formato según el tipo. Devuelve el valor normalizado o null si no es válido.</summary>
    public static string? Normalizar(string tipo, string? valor)
    {
        var texto = valor?.Trim() ?? string.Empty;
        switch (tipo)
        {
            case Pin:
                return texto.Length is >= 4 and <= 16 && texto.All(char.IsAsciiDigit) ? texto : null;
            case Contrasena:
                return texto.Length is >= 1 and <= 100 ? texto : null;
            case Patron:
                var puntos = texto.Split('-', StringSplitOptions.TrimEntries);
                var validos = puntos.Length is >= 4 and <= 9
                    && puntos.All(p => p.Length == 1 && p[0] is >= '1' and <= '9')
                    && puntos.Distinct().Count() == puntos.Length;
                return validos ? string.Join('-', puntos) : null;
            default:
                return null;
        }
    }
}
