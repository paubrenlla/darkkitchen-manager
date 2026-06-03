using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Domain.Products;

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

    public ProductResponse() { }

    [SetsRequiredMembers]
    public ProductResponse(Product product)
    {
        Id = product.Id;
        Code = product.Code;
        Name = product.Name;
        Description = product.Description;
        Price = product.Price;
        Line = product.Line.Name;
        Category = product.Category.Name;
        Images = product.Images.Select(i => i.Url).ToList();
        IsActive = product.IsActive;
    }
}
