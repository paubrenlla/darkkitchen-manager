namespace DarkKitchen.Models.DTOs;

public class ProductResponse
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public required string Line { get; set; }
    public required string Category { get; set; }
    public required List<string> Images { get; set; }
    public bool IsActive { get; set; }
}
