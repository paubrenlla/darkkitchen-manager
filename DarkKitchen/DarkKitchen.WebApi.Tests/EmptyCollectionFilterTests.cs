using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class EmptyCollectionFilterTests
{
    private EmptyCollectionFilter _filter = null!;

    [TestInitialize]
    public void Setup()
    {
        _filter = new EmptyCollectionFilter();
    }

    private static ActionExecutingContext BuildExecutingContext()
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), new object());
    }

    private static ActionExecutedContext BuildExecutedContext(IActionResult result)
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new ActionExecutedContext(actionContext, [], new object()) { Result = result };
    }

    [TestMethod]
    public void OnActionExecuting_ShouldDoNothing()
    {
        var context = BuildExecutingContext();

        _filter.OnActionExecuting(context);

        Assert.IsNull(context.Result);
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
    public void OnActionExecuted_BadRequestResult_ShouldNotModify()
    {
        var context = BuildExecutedContext(new BadRequestObjectResult("error"));

        _filter.OnActionExecuted(context);

        Assert.IsInstanceOfType(context.Result, typeof(BadRequestObjectResult));
    }
}
