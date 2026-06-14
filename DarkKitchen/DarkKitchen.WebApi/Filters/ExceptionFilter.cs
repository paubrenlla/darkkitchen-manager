using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private static readonly Dictionary<Type, HttpStatusCode> _statusCodeMap = new()
    {
        { typeof(ArgumentException),       HttpStatusCode.BadRequest },
        { typeof(NotSupportedException),   HttpStatusCode.BadRequest },   // ← agregar
        { typeof(KeyNotFoundException),    HttpStatusCode.NotFound },
        { typeof(InvalidOperationException), HttpStatusCode.Conflict },
        { typeof(UnauthorizedAccessException), HttpStatusCode.Unauthorized }
    };

    public void OnException(ExceptionContext context)
    {
        var exceptionType = context.Exception.GetType();

        var matched = _statusCodeMap
            .FirstOrDefault(kvp => kvp.Key.IsAssignableFrom(exceptionType));

        if(matched.Key != null)
        {
            context.Result = new ObjectResult(new { error = context.Exception.Message })
                { StatusCode = (int)matched.Value };
        }
        else
        {
            context.Result = new ObjectResult(new { error = "Ocurrió un error inesperado." })
                { StatusCode = (int)HttpStatusCode.InternalServerError };
        }

        context.ExceptionHandled = true;
    }
}
