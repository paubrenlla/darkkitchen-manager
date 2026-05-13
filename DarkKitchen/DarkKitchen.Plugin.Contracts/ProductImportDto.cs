namespace DarkKitchen.Plugin.Contracts;

public class ProductImportDto
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? LineName { get; set; }
    public string? CategoryName { get; set; }
    public decimal Price { get; set; }
    public List<ImageImportDto>? Images { get; set; }
}
