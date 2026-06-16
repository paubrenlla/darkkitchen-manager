using System.Security.Claims;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class OrderStatusAuthorizationFilterTests
{
    private OrderStatusAuthorizationFilter _filter = null!;
    private Mock<HttpContext> _httpContextMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _filter = new OrderStatusAuthorizationFilter();
        _httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
    }

    private ActionExecutingContext BuildExecutingContext(string status, string? callerRole)
    {
        List<Claim> claims = callerRole != null
            ? [new Claim(ClaimTypes.Role, callerRole)]
            : [];

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        _httpContextMock.Setup(h => h.User).Returns(principal);

        var args = new Dictionary<string, object?>
        {
            ["request"] = new OrderStatusUpdateRequest { Status = status }
        };

        var actionContext = new ActionContext(
            _httpContextMock.Object,
            new RouteData(),
            new ActionDescriptor());

        return new ActionExecutingContext(actionContext, [], args, new object());
    }

    [TestMethod]
    public void OnActionExecuting_PreparadorMarkAsPreparado_ShouldNotForbid()
    {
        var context = BuildExecutingContext("Preparado", "Preparador");

        _filter.OnActionExecuting(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void OnActionExecuting_AdministrativoMarkAsPreparado_ShouldNotForbid()
    {
        var context = BuildExecutingContext("Preparado", "Administrativo");

        _filter.OnActionExecuting(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void OnActionExecuting_ClienteMarkAsPreparado_ShouldForbid()
    {
        var context = BuildExecutingContext("Preparado", "Cliente");

        _filter.OnActionExecuting(context);

        Assert.IsInstanceOfType(context.Result, typeof(ForbidResult));
    }

    [TestMethod]
    public void OnActionExecuting_AdministrativoMarkAsCancelado_ShouldNotForbid()
    {
        var context = BuildExecutingContext("Cancelado", "Administrativo");

        _filter.OnActionExecuting(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void OnActionExecuting_PreparadorMarkAsCancelado_ShouldForbid()
    {
        var context = BuildExecutingContext("Cancelado", "Preparador");

        _filter.OnActionExecuting(context);

        Assert.IsInstanceOfType(context.Result, typeof(ForbidResult));
    }

    [TestMethod]
    public void OnActionExecuting_PreparadorMarkAsDemorado_ShouldNotForbid()
    {
        var context = BuildExecutingContext("Demorado", "Preparador");

        _filter.OnActionExecuting(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void OnActionExecuting_UnknownStatus_ShouldForbid()
    {
        var context = BuildExecutingContext("EstadoInvalido", "Administrativo");

        _filter.OnActionExecuting(context);

        Assert.IsInstanceOfType(context.Result, typeof(ForbidResult));
    }

    [TestMethod]
    public void OnActionExecuting_NullRole_ShouldForbid()
    {
        var context = BuildExecutingContext("Preparado", null);

        _filter.OnActionExecuting(context);

        Assert.IsInstanceOfType(context.Result, typeof(ForbidResult));
    }

    [TestMethod]
    public void OnActionExecuting_NoRequestInArgs_ShouldNotForbid()
    {
        _httpContextMock.Setup(h => h.User).Returns(new ClaimsPrincipal());

        var actionContext = new ActionContext(
            _httpContextMock.Object,
            new RouteData(),
            new ActionDescriptor());

        var context = new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), new object());

        _filter.OnActionExecuting(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void OnActionExecuted_ShouldNotModifyContext()
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var result = new OkObjectResult("algo");
        var context = new ActionExecutedContext(actionContext, [], new object()) { Result = result };

        _filter.OnActionExecuted(context);

        Assert.AreEqual(result, context.Result);
    }
}
