using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Models.DTOs;

public class OrderCreateResponse(Order order)
{
    public Guid Id { get; set; } = order.Id;
    public Guid ClientId { get; set; } = order.ClientId;
    public int OrderNumber { get; set; } = order.OrderNumber ?? 0;
    public decimal Subtotal { get; set; } = order.Subtotal;
    public decimal ShippingCost { get; set; } = order.ShippingCost;
    public decimal Total { get; set; } = order.Total;
}
