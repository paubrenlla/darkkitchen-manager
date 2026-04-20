using DarkKitchen.Domain.Orders;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.Converters;
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
            if(!Enum.TryParse<DeliveryType>(request.DeliveryType, true, out var deliveryType))
            {
                return BadRequest(new { error = "Tipo de entrega inválido." });
            }

            var address = new Address(
                request.Address.Street,
                request.Address.Number,
                request.Address.Apartment,
                request.Address.City,
                request.Address.Country);

            var items = request.Items.Select(i =>
                new OrderItem(i.ProductId, i.Quantity, 0m)).ToList();

            var order = _orderService.CreateOrder(clientId, address, deliveryType, items);
            var response = Converter.ToOrderCreateResponse(order);

            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
