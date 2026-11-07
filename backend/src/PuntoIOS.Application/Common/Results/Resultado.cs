namespace PuntoIOS.Application.Common.Results;

public enum TipoError
{
    Validacion,
    NoEncontrado,
    Conflicto,
}

/// <summary>Error de negocio. <c>Datos</c> agrega información útil para quien consume la API (p. ej. coincidencias).</summary>
public sealed record Error(
    TipoError Tipo,
    string Codigo,
    string Mensaje,
    IReadOnlyDictionary<string, object?>? Datos = null);

/// <summary>Resultado de un caso de uso: un valor o un error de negocio (sin usar excepciones para el flujo normal).</summary>
public sealed class Resultado<T>
{
    private Resultado(T? valor, Error? error)
    {
        Valor = valor;
        Error = error;
    }

    public T? Valor { get; }

    public Error? Error { get; }

    public bool EsExitoso => Error is null;

    public static Resultado<T> Ok(T valor) => new(valor, null);

    public static Resultado<T> Falla(Error error) => new(default, error);

    public static Resultado<T> Falla(TipoError tipo, string codigo, string mensaje) =>
        new(default, new Error(tipo, codigo, mensaje));
}
