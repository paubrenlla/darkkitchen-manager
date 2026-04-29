namespace DarkKitchen.Models.DTOs;

public class TopProductResponse
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public int QuantitySold { get; set; }
    public required List<string> Images { get; set; }
}
