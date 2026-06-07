using System.Security.Claims;
using DarkKitchen.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

public class AuthorizationFilter : Attribute, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.ActionArguments.Values
            .OfType<UserCreateRequest>()
            .FirstOrDefault();

        if(request?.Role == null)
        {
            return;
        }

        var callerRole = context.HttpContext.User
            .FindFirst(ClaimTypes.Role)?.Value;

        if(callerRole != "Administrativo")
        {
            context.Result = new ForbidResult();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
