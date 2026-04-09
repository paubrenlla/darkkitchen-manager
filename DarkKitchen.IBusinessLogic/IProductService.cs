using DarkKitchen.Domain;

namespace DarkKitchen.IBusinessLogic;

public interface IProductService
{
    IEnumerable<Product> GetProducts(string? name, string? line, string? category);
}
