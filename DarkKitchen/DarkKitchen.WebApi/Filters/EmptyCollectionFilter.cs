using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

public class EmptyCollectionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if(context.Result is OkObjectResult { Value: IEnumerable<object> collection } &&
           !collection.Any())
        {
            context.Result = new NoContentResult();
        }
    }
}
