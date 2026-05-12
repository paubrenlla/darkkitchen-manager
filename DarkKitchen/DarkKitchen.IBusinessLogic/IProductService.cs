using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IProductService
{
    IEnumerable<ProductResponse> GetProducts(string? name, string? line, string? category);
    ProductResponse CreateProduct(ProductCreateRequest request, string currentUser);
    ProductResponse UpdateProduct(Guid id, ProductUpdateRequest request, string currentUser);
    IEnumerable<ProductResponse> ImportProducts(string importerName, string filePath, string currentUser);
}
