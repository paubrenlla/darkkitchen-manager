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
    public IActionResult CreateOrder([FromBody] OrderCreateRequest request, [FromHeader(Name = "X-Client-Id")] Guid clientId)
    {
        try
        {
            var response = _orderService.CreateOrder(clientId, request);
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
            switch(request.Status.ToLower())
            {
                case "cancelado":
                    _orderService.Cancel(id);
                    break;
                case "entregado":
                    _orderService.Deliver(id);
                    break;
                case "preparado":
                    _orderService.Prepare(id);
                    break;
                case "encamino":
                    _orderService.Ship(id);
                    break;
                case "noentregado":
                    _orderService.NotDelivered(id);
                    break;
                default:
                    return BadRequest(new { error = $"Estado '{request.Status}' no válido." });
            }

            var response = _orderService.GetOrderById(id);
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

    [HttpGet("{id}")]
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

    [HttpGet]
    public IActionResult GetOrders(
        [FromHeader(Name = "X-Client-Id")] Guid clientId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? status)
    {
        var orders = _orderService.GetOrdersByClient(clientId, fromDate, toDate, status);
        return Ok(orders);
    }
}
