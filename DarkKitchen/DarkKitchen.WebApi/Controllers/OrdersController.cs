using System.Security.Claims;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Filters;
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
    [OrderStatusAuthorizationFilter]
    public IActionResult UpdateStatus(Guid id, [FromBody] OrderStatusUpdateRequest request)
    {
        _orderService.UpdateOrderStatus(id, request.Status);
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
        if (toDate.HasValue && toDate.Value.TimeOfDay == TimeSpan.Zero)
        {
            toDate = toDate.Value.Date.AddDays(1).AddSeconds(-1);
        }

        var callerRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var callerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var filter = new OrderFilter { From = fromDate, To = toDate, State = status, Address = address };
        var orders = _orderService.GetOrders(callerId, callerRole, filter);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Preparador,Administrativo")]
    public IActionResult GetOrderDetail(Guid id)
    {
        var order = _orderService.GetOrderById(id);
        return Ok(new OrderDetailResponse(order));
    }
}
