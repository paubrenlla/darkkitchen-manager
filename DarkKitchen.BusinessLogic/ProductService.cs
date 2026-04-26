using DarkKitchen.Domain.Products;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class ProductService(IProductRepository productRepository) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;

    public IEnumerable<ProductResponse> GetProducts(string? name, string? line, string? category)
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

        return products.Select(Converter.ToProductResponse);
    }

    public ProductResponse CreateProduct(ProductCreateRequest request)
    {
        var product = Converter.ToProduct(request);
        _productRepository.Add(product);
        return Converter.ToProductResponse(product);
    }

    public ProductResponse UpdateProduct(Guid id, ProductUpdateRequest request)
    {
        Product product = _productRepository.GetById(id)
                          ?? throw new KeyNotFoundException($"Producto {id} no encontrado.");

        var line = new ProductLine(request.Line);
        var category = new ProductCategory(request.Category);
        var images = request.Images
            .Select(i => new ProductImage(i.Url, i.SizeInBytes))
            .ToList();

        product.UpdateDetails(request.Name, request.Description, line, category, request.Price, images);

        if(request.IsActive.HasValue)
        {
            if(request.IsActive.Value)
            {
                product.Activate();
            }
            else
            {
                product.Deactivate();
            }
        }

        _productRepository.Update(id, product);
        return Converter.ToProductResponse(product);
    }
}
