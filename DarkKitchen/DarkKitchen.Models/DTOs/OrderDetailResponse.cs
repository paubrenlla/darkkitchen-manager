using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Models.DTOs;

public class OrderAddressDetailDto
{
    public required string Street { get; set; }
    public required string Number { get; set; }
    public string? Apartment { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
}

public class OrderItemDetailDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? AppliedPromotion { get; set; }
    public decimal ItemTotal { get; set; }
}

public class OrderDetailResponse
{
    public Guid Id { get; set; }
    public int? OrderNumber { get; set; }
    public Guid ClientId { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Status { get; set; }
    public required string DeliveryType { get; set; }
    public required OrderAddressDetailDto Address { get; set; }
    public required List<OrderItemDetailDto> Items { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    [SetsRequiredMembers]
    public OrderDetailResponse(Order order)
    {
        Id = order.Id;
        OrderNumber = order.OrderNumber;
        ClientId = order.ClientId;
        CreatedAt = order.CreatedAt;
        Status = order.State.ToString();
        DeliveryType = order.Type;
        Address = new OrderAddressDetailDto
        {
            Street = order.DeliveryAddress.Street,
            Number = order.DeliveryAddress.Number,
            Apartment = order.DeliveryAddress.Apartment,
            City = order.DeliveryAddress.City,
            Country = order.DeliveryAddress.Country
        };
        Items = order.Items.Select(i => new OrderItemDetailDto
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            Price = i.Price,
            AppliedPromotion = i.AppliedPromotionName,
            ItemTotal = i.CalculateItemTotal()
        }).ToList();
        Subtotal = order.Subtotal;
        ShippingCost = order.ShippingCost;
        Tax = order.Tax;
        Total = order.Total;
    }
}
