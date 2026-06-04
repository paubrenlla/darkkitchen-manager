using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Models.DTOs;

public class OrderDetailResponse
{
    public Guid Id { get; set; }
    public int? OrderNumber { get; set; }
    public Guid ClientId { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Status { get; set; }
    public required List<OrderItemDetailDto> Items { get; set; }
    public decimal Total { get; set; }

    public OrderDetailResponse()
    {
    }

    [SetsRequiredMembers]
    public OrderDetailResponse(Order order)
    {
        Id = order.Id;
        OrderNumber = order.OrderNumber;
        ClientId = order.ClientId;
        CreatedAt = order.CreatedAt;
        Status = order.State.ToString();
        Items = order.Items.Select(i => new OrderItemDetailDto
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            Price = i.Price,
            AppliedPromotion = i.AppliedPromotionName,
            ItemTotal = i.CalculateItemTotal()
        }).ToList();
        Total = order.Total;
    }
}

public class OrderItemDetailDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? AppliedPromotion { get; set; }
    public decimal ItemTotal { get; set; }
}
