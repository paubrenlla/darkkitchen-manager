using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic;

public class ProductService(IProductRepository productRepository) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;

    public IEnumerable<Product> GetProducts(string? name, string? line, string? category)
    {
        var products = _productRepository.GetAll();

        if(!string.IsNullOrWhiteSpace(name))
        {
            products = products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if(!string.IsNullOrWhiteSpace(line))
        {
            products = products.Where(p => p.Line.Name.Equals(line, StringComparison.OrdinalIgnoreCase));
        }

        if(!string.IsNullOrWhiteSpace(category))
        {
            products = products.Where(p => p.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        return products;
    }
}
