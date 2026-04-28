using System.Net;
using System.Reflection;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class ExceptionFilterTests
{
    private ExceptionFilter _filter = null!;

    private ExceptionContext CreateContext(Exception? ex)
    {
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData(),
            ActionDescriptor = new ControllerActionDescriptor()
        };
        return new ExceptionContext(actionContext, new List<IFilterMetadata>()) { Exception = ex! };
    }

    private static string GetErrorMessage(ObjectResult result)
    {
        PropertyInfo? property = result.Value!.GetType().GetProperty("error");
        return property!.GetValue(result.Value)!.ToString()!;
    }

    [TestInitialize]
    public void Setup()
    {
        _filter = new ExceptionFilter();
    }

    [TestMethod]
    public void OnException_UnauthorizedAccessException_Returns401()
    {
        ExceptionContext context = CreateContext(new UnauthorizedAccessException("No autorizado"));

        _filter.OnException(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.AreEqual("No autorizado", GetErrorMessage(result));
    }

    [TestMethod]
    public void OnException_ArgumentException_Returns400()
    {
        ExceptionContext context = CreateContext(new ArgumentException("Argumento inválido"));

        _filter.OnException(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.AreEqual("Argumento inválido", GetErrorMessage(result));
    }

    [TestMethod]
    public void OnException_InvalidOperationException_Returns400()
    {
        ExceptionContext context = CreateContext(new InvalidOperationException("Operación inválida"));

        _filter.OnException(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.AreEqual("Operación inválida", GetErrorMessage(result));
    }

    [TestMethod]
    public void OnException_KeyNotFoundException_Returns404()
    {
        ExceptionContext context = CreateContext(new KeyNotFoundException("No encontrado"));

        _filter.OnException(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        Assert.AreEqual("No encontrado", GetErrorMessage(result));
    }

    [TestMethod]
    public void OnException_UnknownException_Returns500()
    {
        ExceptionContext context = CreateContext(new Exception("Error inesperado"));

        _filter.OnException(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.AreEqual("Ocurrió un error inesperado.", GetErrorMessage(result));
    }

    [TestMethod]
    public void OnException_NullException_Returns500()
    {
        ExceptionContext context = CreateContext(null);

        _filter.OnException(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.AreEqual("Ocurrió un error inesperado.", GetErrorMessage(result));
    }

    [TestMethod]
    public void OnException_SiempreMarcaExceptionComoHandled()
    {
        ExceptionContext context = CreateContext(new Exception("cualquier error"));

        _filter.OnException(context);

        Assert.IsTrue(context.ExceptionHandled);
    }
}
