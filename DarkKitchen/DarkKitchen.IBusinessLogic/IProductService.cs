using DarkKitchen.Domain.Products;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IProductService
{
    IEnumerable<Product> GetProducts(string? name, string? line, string? category);
    Product CreateProduct(ProductCreateRequest request, string currentUser);
    Product UpdateProduct(Guid id, ProductUpdateRequest request, string currentUser);
    ProductImportResponse ImportProducts(string importerName, string filePath, string currentUser);
}
