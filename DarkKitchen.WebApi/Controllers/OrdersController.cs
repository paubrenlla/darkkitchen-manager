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
        try
        {
            var clientId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            OrderCreateResponse response = _orderService.CreateOrder(clientId, request);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    public IActionResult UpdateStatus(Guid id, [FromBody] OrderStatusUpdateRequest request)
    {
        try
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

                case "cancelado":
                    if(callerRole != "Administrativo")
                    {
                        return Forbid();
                    }

                    _orderService.Cancel(id);
                    break;

                case "encamino":
                case "entregado":
                case "noentregado":
                    if(callerRole != "Preparador")
                    {
                        return Forbid();
                    }

                    if(request.Status.ToLower() == "encamino")
                    {
                        _orderService.Ship(id);
                    }
                    else if(request.Status.ToLower() == "entregado")
                    {
                        _orderService.Deliver(id);
                    }
                    else
                    {
                        _orderService.NotDelivered(id);
                    }

                    break;

                default:
                    return BadRequest(new { error = $"Estado '{request.Status}' no válido." });
            }

            OrderDetailResponse response = _orderService.GetOrderById(id);
            return Ok(response);
        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult GetOrders(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? status,
        [FromQuery] string? city)
    {
        var callerRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var callerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if(callerRole == "Preparador")
        {
            if(fromDate == null || toDate == null)
            {
                return BadRequest(new { error = "El rango de fechas es obligatorio para el preparador." });
            }

            IEnumerable<OrderListResponse> preparadorOrders = _orderService.GetOrdersByStatus(fromDate.Value, toDate.Value, status, city);
            return Ok(preparadorOrders);
        }

        IEnumerable<OrderListResponse> clientOrders = _orderService.GetOrdersByClient(callerId, fromDate, toDate, status);
        return Ok(clientOrders);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Preparador,Administrativo")]
    public IActionResult GetOrderDetail(Guid id)
    {
        try
        {
            var response = _orderService.GetOrderById(id);
            return Ok(response);
        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
