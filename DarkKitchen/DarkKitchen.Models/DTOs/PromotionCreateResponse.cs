namespace DarkKitchen.Models.DTOs;

public class PromotionCreateResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> Products { get; set; } = [];
}
