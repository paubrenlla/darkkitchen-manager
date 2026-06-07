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

    [TestMethod]
    public void OnActionExecuted_ShouldDoNothing()
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var okResult = new OkResult();
        var context = new ActionExecutedContext(actionContext, [], new object()) { Result = okResult };

        _filter.OnActionExecuted(context);

        Assert.AreEqual(okResult, context.Result);
    }
}
