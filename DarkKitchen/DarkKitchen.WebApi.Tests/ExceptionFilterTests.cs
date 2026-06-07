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

    [TestMethod]
    public void OnException_ArgumentException_ShouldReturn400()
    {
        _context.Exception = new ArgumentException("argumento inválido");

        _filter.OnException(_context);

        var result = _context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.IsTrue(_context.ExceptionHandled);
    }

    [TestMethod]
    public void OnException_KeyNotFoundException_ShouldReturn404()
    {
        _context.Exception = new KeyNotFoundException("no encontrado");

        _filter.OnException(_context);

        var result = _context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        Assert.IsTrue(_context.ExceptionHandled);
    }

    [TestMethod]
    public void OnException_InvalidOperationException_ShouldReturn409()
    {
        _context.Exception = new InvalidOperationException("operación inválida");

        _filter.OnException(_context);

        var result = _context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.Conflict, result.StatusCode);
        Assert.IsTrue(_context.ExceptionHandled);
    }

    [TestMethod]
    public void OnException_UnauthorizedAccessException_ShouldReturn401()
    {
        _context.Exception = new UnauthorizedAccessException("no autorizado");

        _filter.OnException(_context);

        var result = _context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.IsTrue(_context.ExceptionHandled);
    }

    [TestMethod]
    public void OnException_UnknownException_ShouldReturn500()
    {
        _context.Exception = new Exception("error inesperado");

        _filter.OnException(_context);

        var result = _context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.IsTrue(_context.ExceptionHandled);
    }
}
