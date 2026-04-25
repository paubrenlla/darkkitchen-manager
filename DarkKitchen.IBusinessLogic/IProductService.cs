using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IProductService
{
    IEnumerable<ProductResponse> GetProducts(string? name, string? line, string? category);
    ProductResponse CreateProduct(ProductCreateRequest request);
}
