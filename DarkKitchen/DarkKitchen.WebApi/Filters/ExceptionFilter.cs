using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly Dictionary<Type, IActionResult> _exceptionMap = new()
    {
        {
            typeof(ArgumentException),
            new ObjectResult(new { error = "Los argumentos provistos no son válidos." })
                { StatusCode = (int)HttpStatusCode.BadRequest }
        },
        {
            typeof(KeyNotFoundException),
            new ObjectResult(new { error = "El recurso solicitado no fue encontrado." })
                { StatusCode = (int)HttpStatusCode.NotFound }
        },
        {
            typeof(InvalidOperationException),
            new ObjectResult(new { error = "La operación no es válida en el estado actual." })
                { StatusCode = (int)HttpStatusCode.Conflict }
        },
        {
            typeof(UnauthorizedAccessException),
            new ObjectResult(new { error = "No está autorizado para realizar esta acción." })
                { StatusCode = (int)HttpStatusCode.Unauthorized }
        }
    };

    public void OnException(ExceptionContext context)
    {
        if(_exceptionMap.TryGetValue(context.Exception.GetType(), out var result))
        {
            context.Result = result;
        }
        else
        {
            context.Result = new ObjectResult(new { error = "Ocurrió un error inesperado." })
            { StatusCode = (int)HttpStatusCode.InternalServerError };
        }

        context.ExceptionHandled = true;
    }
}
