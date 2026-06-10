using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Domain.Orders.Delivery;

namespace DarkKitchen.Models.DTOs;

public class ShippingTypeResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public decimal Cost { get; set; }

    [SetsRequiredMembers]
    public ShippingTypeResponse(ShippingType shippingType)
    {
        Id = shippingType.Id;
        Name = shippingType.Name;
        Cost = shippingType.Cost;
    }
}
