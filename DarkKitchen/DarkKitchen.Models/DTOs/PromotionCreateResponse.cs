using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Domain.Promotions;

namespace DarkKitchen.Models.DTOs;

[method: SetsRequiredMembers]
public class PromotionCreateResponse(Promotion promotion)
{
    public Guid Id { get; set; } = promotion.Id;
    public required string Name { get; set; } = promotion.Name;
    public int DiscountPercentage { get; set; } = promotion.DiscountPercentage;
    public DateTime StartDate { get; set; } = promotion.StartDate;
    public DateTime EndDate { get; set; } = promotion.EndDate;
    public List<string> Products { get; set; } = promotion.Products.Select(p => p.Code).ToList();
}
