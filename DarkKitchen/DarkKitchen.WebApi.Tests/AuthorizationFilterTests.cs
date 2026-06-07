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
public class AuthorizationFilterTests
{
    private AuthorizationFilter _filter = null!;
    private Mock<HttpContext> _httpContextMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _filter = new AuthorizationFilter();
        _httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
    }

    private ActionExecutingContext BuildExecutingContext(IDictionary<string, object?> args)
    {
        var actionContext = new ActionContext(
            _httpContextMock.Object,
            new RouteData(),
            new ActionDescriptor());
        return new ActionExecutingContext(actionContext, [], args, new object());
    }

    private static UserCreateRequest BuildRequest(string? role = null)
    {
        return new UserCreateRequest
        {
            Name = "Juan",
            Surname = "Perez",
            Email = "juan@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094123456",
            Password = "Valid1Password!@",
            Role = role
        };
    }

    [TestMethod]
    public void OnActionExecuting_RequestWithNullRole_ShouldNotForbid()
    {
        _httpContextMock.Setup(h => h.User).Returns(new ClaimsPrincipal());

        var args = new Dictionary<string, object?> { ["request"] = BuildRequest(role: null) };
        var context = BuildExecutingContext(args);

        _filter.OnActionExecuting(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void OnActionExecuting_RequestWithRoleAndAdminCaller_ShouldNotForbid()
    {
        var claims = new List<Claim> { new(ClaimTypes.Role, "Administrativo") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        _httpContextMock.Setup(h => h.User).Returns(principal);

        var args = new Dictionary<string, object?> { ["request"] = BuildRequest(role: "Preparador") };
        var context = BuildExecutingContext(args);

        _filter.OnActionExecuting(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void OnActionExecuting_RequestWithRoleAndNonAdminCaller_ShouldForbid()
    {
        var claims = new List<Claim> { new(ClaimTypes.Role, "Cliente") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        _httpContextMock.Setup(h => h.User).Returns(principal);

        var args = new Dictionary<string, object?> { ["request"] = BuildRequest(role: "Preparador") };
        var context = BuildExecutingContext(args);

        _filter.OnActionExecuting(context);

        Assert.IsInstanceOfType(context.Result, typeof(ForbidResult));
    }

    [TestMethod]
    public void OnActionExecuting_RequestWithRoleAndAnonymousCaller_ShouldForbid()
    {
        _httpContextMock.Setup(h => h.User).Returns(new ClaimsPrincipal());

        var args = new Dictionary<string, object?> { ["request"] = BuildRequest(role: "Preparador") };
        var context = BuildExecutingContext(args);

        _filter.OnActionExecuting(context);

        Assert.IsInstanceOfType(context.Result, typeof(ForbidResult));
    }
}

[TestClass]
public class EmptyCollectionFilterTests
{
    private EmptyCollectionFilter _filter = null!;

    [TestInitialize]
    public void Setup()
    {
        _filter = new EmptyCollectionFilter();
    }

    private static ActionExecutedContext BuildExecutedContext(IActionResult result)
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());
        return new ActionExecutedContext(actionContext, [], new object()) { Result = result };
    }

    [TestMethod]
    public void OnActionExecuted_EmptyCollection_ShouldReturnNoContent()
    {
        var context = BuildExecutedContext(new OkObjectResult(new List<object>()));

        _filter.OnActionExecuted(context);

        Assert.IsInstanceOfType(context.Result, typeof(NoContentResult));
    }

    [TestMethod]
    public void OnActionExecuted_NonEmptyCollection_ShouldKeepOk()
    {
        var context = BuildExecutedContext(new OkObjectResult(new List<object> { new() }));

        _filter.OnActionExecuted(context);

        Assert.IsInstanceOfType(context.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void OnActionExecuted_NonCollectionResult_ShouldNotModify()
    {
        var context = BuildExecutedContext(new OkObjectResult("string result"));

        _filter.OnActionExecuted(context);

        Assert.IsInstanceOfType(context.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void OnActionExecuted_NoContentResult_ShouldNotModify()
    {
        var context = BuildExecutedContext(new NoContentResult());

        _filter.OnActionExecuted(context);

        Assert.IsInstanceOfType(context.Result, typeof(NoContentResult));
    }

    [TestMethod]
    public void OnActionExecuted_NonOkObjectResult_ShouldNotModify()
    {
        var context = BuildExecutedContext(new BadRequestObjectResult("error"));

        _filter.OnActionExecuted(context);

        Assert.IsInstanceOfType(context.Result, typeof(BadRequestObjectResult));
    }
}
