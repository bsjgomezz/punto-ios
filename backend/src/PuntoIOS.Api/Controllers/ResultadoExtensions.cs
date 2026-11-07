using Microsoft.AspNetCore.Mvc;
using PuntoIOS.Application.Common.Results;

namespace PuntoIOS.Api.Controllers;

public static class ResultadoExtensions
{
    /// <summary>Convierte un error de negocio en una respuesta ProblemDetails con el código HTTP correspondiente.</summary>
    public static ObjectResult ToProblem(this Error error)
    {
        var status = error.Tipo switch
        {
            TipoError.NoEncontrado => StatusCodes.Status404NotFound,
            TipoError.Conflicto => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest,
        };

        var problem = new ProblemDetails { Status = status, Title = error.Mensaje };
        problem.Extensions["codigo"] = error.Codigo;
        return new ObjectResult(problem) { StatusCode = status };
    }
}
