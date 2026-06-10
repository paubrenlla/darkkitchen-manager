using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Domain.Orders.Delivery;

namespace DarkKitchen.Models.DTOs;

[method: SetsRequiredMembers]
public class ShippingTypeResponse(ShippingType shippingType)
{
    public Guid Id { get; set; } = shippingType.Id;
    public required string Name { get; set; } = shippingType.Name;
    public decimal Cost { get; set; } = shippingType.Cost;
}
