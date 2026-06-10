using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Models.DTOs;

public class OrderCreateResponse
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public int OrderNumber { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }

    public OrderCreateResponse(Order order)
    {
        Id = order.Id;
        ClientId = order.ClientId;
        OrderNumber = order.OrderNumber ?? 0;
        Subtotal = order.Subtotal;
        ShippingCost = order.ShippingCost;
        Total = order.Total;
    }
}
