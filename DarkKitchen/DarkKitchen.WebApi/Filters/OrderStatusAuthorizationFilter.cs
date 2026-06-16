using System.Security.Claims;
using DarkKitchen.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

public class OrderStatusAuthorizationFilter : Attribute, IActionFilter
{
    private static readonly Dictionary<string, string[]> _allowedRoles = new()
    {
        { "preparado",   ["Preparador", "Administrativo"] },
        { "demorado",    ["Preparador"] },
        { "cancelado",   ["Administrativo"] },
        { "encamino",    ["Preparador"] },
        { "entregado",   ["Preparador"] },
        { "noentregado", ["Preparador"] },
    };

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.ActionArguments.Values
            .OfType<OrderStatusUpdateRequest>()
            .FirstOrDefault();

        if(request == null)
        {
            return;
        }

        var status = request.Status.ToLower();
        var callerRole = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

        if(!_allowedRoles.TryGetValue(status, out var roles) || !roles.Contains(callerRole))
        {
            context.Result = new ForbidResult();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
