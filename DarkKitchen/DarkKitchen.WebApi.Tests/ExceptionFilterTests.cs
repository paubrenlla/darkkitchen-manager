using System.Net;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class ExceptionFilterTests
{
    private ExceptionFilter _filter = null!;
    private ExceptionContext _context = null!;

    [TestInitialize]
    public void Setup()
    {
        _filter = new ExceptionFilter();
        _context = new ExceptionContext(
            new ActionContext(
                new Mock<HttpContext>(MockBehavior.Strict).Object,
                new RouteData(),
                new ActionDescriptor()),
            []);
    }

    private string? GetErrorMessage() =>
        _context.Result is ObjectResult result
            ? result.Value?.GetType().GetProperty("error")?.GetValue(result.Value) as string
            : null;

    [TestMethod]
    public void OnException_ArgumentException_ShouldReturn400WithMessage()
    {
        _context.Exception = new ArgumentException("argumento inválido");

        _filter.OnException(_context);

        var result = _context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.AreEqual("argumento inválido", GetErrorMessage());
        Assert.IsTrue(_context.ExceptionHandled);
    }

    [TestMethod]
    public void OnException_KeyNotFoundException_ShouldReturn404WithMessage()
    {
        _context.Exception = new KeyNotFoundException("recurso no encontrado");

        _filter.OnException(_context);

        var result = _context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        Assert.AreEqual("recurso no encontrado", GetErrorMessage());
        Assert.IsTrue(_context.ExceptionHandled);
    }

    [TestMethod]
    public void OnException_InvalidOperationException_ShouldReturn409WithMessage()
    {
        _context.Exception = new InvalidOperationException("operación inválida");

        _filter.OnException(_context);

        var result = _context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.Conflict, result.StatusCode);
        Assert.AreEqual("operación inválida", GetErrorMessage());
        Assert.IsTrue(_context.ExceptionHandled);
    }

    [TestMethod]
    public void OnException_UnauthorizedAccessException_ShouldReturn401WithMessage()
    {
        _context.Exception = new UnauthorizedAccessException("no autorizado");

        _filter.OnException(_context);

        var result = _context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.AreEqual("no autorizado", GetErrorMessage());
        Assert.IsTrue(_context.ExceptionHandled);
    }

    [TestMethod]
    public void OnException_UnknownException_ShouldReturn500WithGenericMessage()
    {
        _context.Exception = new Exception("error interno detallado");

        _filter.OnException(_context);

        var result = _context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.AreEqual("Ocurrió un error inesperado.", GetErrorMessage());
        Assert.IsTrue(_context.ExceptionHandled);
    }

    [TestMethod]
    public void OnException_NotSupportedException_ShouldReturn400WithMessage()
    {
        _context.Exception = new NotSupportedException("El país con prefijo '+100' aún no está soportado.");

        _filter.OnException(_context);

        var result = _context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.AreEqual("El país con prefijo '+100' aún no está soportado.", GetErrorMessage());
        Assert.IsTrue(_context.ExceptionHandled);
    }
}
