using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Domain.Promotions;

namespace DarkKitchen.Models.DTOs;

public class PromotionCreateResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> Products { get; set; } = [];

    [SetsRequiredMembers]
    public PromotionCreateResponse(Promotion promotion)
    {
        Id = promotion.Id;
        Name = promotion.Name;
        DiscountPercentage = promotion.DiscountPercentage;
        StartDate = promotion.StartDate;
        EndDate = promotion.EndDate;
        Products = promotion.Products.Select(p => p.Code).ToList();
    }
}
