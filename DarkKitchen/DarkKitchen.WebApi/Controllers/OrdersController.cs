using System.Security.Claims;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    [HttpPost]
    [Authorize(Roles = "Cliente")]
    public IActionResult CreateOrder([FromBody] OrderCreateRequest request)
    {
        var clientId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var order = _orderService.CreateOrder(clientId, request);
        return StatusCode(StatusCodes.Status201Created, new OrderCreateResponse(order));
    }

    [HttpPatch("{id}/status")]
    public IActionResult UpdateStatus(Guid id, [FromBody] OrderStatusUpdateRequest request)
    {
        var callerRole = User.FindFirst(ClaimTypes.Role)?.Value;

        switch(request.Status.ToLower())
        {
            case "preparado":
                if(callerRole != "Preparador" && callerRole != "Administrativo")
                {
                    return Forbid();
                }

                _orderService.Prepare(id);
                break;

            case "demorado":
                if(callerRole != "Preparador")
                {
                    return Forbid();
                }

                _orderService.Delay(id);
                break;

            case "cancelado":
                if(callerRole != "Administrativo")
                {
                    return Forbid();
                }

                _orderService.Cancel(id);
                break;

            case "encamino":
                if(callerRole != "Preparador")
                {
                    return Forbid();
                }

                _orderService.Ship(id);
                break;

            case "entregado":
                if(callerRole != "Preparador")
                {
                    return Forbid();
                }

                _orderService.Deliver(id);
                break;

            case "noentregado":
                if(callerRole != "Preparador")
                {
                    return Forbid();
                }

                _orderService.NotDelivered(id);
                break;

            default:
                return BadRequest(new { error = $"Estado '{request.Status}' no válido." });
        }

        var order = _orderService.GetOrderById(id);
        return Ok(new OrderDetailResponse(order));
    }

    [HttpGet]
    public IActionResult GetOrders(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? status,
        [FromQuery] string? address)
    {
        var callerRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var callerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if(callerRole == "Preparador")
        {
            if(fromDate == null || toDate == null)
            {
                return BadRequest(new { error = "El rango de fechas es obligatorio para el preparador." });
            }

            var filter = new OrderFilter { From = fromDate, To = toDate, State = status, Address = address };
            var orders = _orderService.GetOrdersByStatus(filter);
            return orders.Any() ? Ok(orders) : NoContent();
        }

        var clientFilter = new OrderFilter { From = fromDate, To = toDate, State = status };
        var clientOrders = _orderService.GetOrdersByClient(callerId, clientFilter);
        return clientOrders.Any() ? Ok(clientOrders) : NoContent();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Preparador,Administrativo")]
    public IActionResult GetOrderDetail(Guid id)
    {
        var order = _orderService.GetOrderById(id);
        return Ok(new OrderDetailResponse(order));
    }
}
