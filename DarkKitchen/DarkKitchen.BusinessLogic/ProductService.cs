using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Products;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class ProductService(IProductRepository productRepository, IDomainEventPublisher eventPublisher)
    : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IDomainEventPublisher _eventPublisher = eventPublisher;

    public IEnumerable<ProductResponse> GetProducts(string? name, string? line, string? category)
    {
        IEnumerable<Product> products = _productRepository.GetAll();

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

    public ProductResponse CreateProduct(ProductCreateRequest request, string currentUser)
    {
        if(_productRepository.GetAll().Any(p => p.Code == request.Code))
        {
            throw new ArgumentException($"Product with code {request.Code} already exists.");
        }

        var product = Converter.ToProduct(request);
        _productRepository.Add(product);
        var domainEvent = new EntityCreatedEvent<Product>
        {
            EntityId = product.Id,
            EntityName = nameof(Product),
            ResponsibleUser = currentUser,
            NewState = product
        };
        _eventPublisher.Publish(domainEvent);
        return Converter.ToProductResponse(product);
    }

    public ProductResponse UpdateProduct(Guid id, ProductUpdateRequest request, string currentUser)
    {
        Product product = _productRepository.GetById(id)
                          ?? throw new KeyNotFoundException($"Producto {id} no encontrado.");

        Product oldProduct = product.Clone();

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

        var domainEvent = new EntityModifiedEvent<Product>
        {
            EntityId = id,
            EntityName = nameof(Product),
            ResponsibleUser = currentUser,
            OldState = oldProduct,
            NewState = product
        };

        _eventPublisher.Publish(domainEvent);

        if(oldProduct.IsActive && !product.IsActive)
        {
            var deactivationEvent = new EntityDeactivatedEvent<Product>
            {
                EntityId = id,
                EntityName = nameof(Product),
                ResponsibleUser = currentUser,
                OldState = oldProduct
            };
            _eventPublisher.Publish(deactivationEvent);
        }
        else if(!oldProduct.IsActive && product.IsActive)
        {
            var activationEvent = new EntityActivatedEvent<Product>
            {
                EntityId = id,
                EntityName = nameof(Product),
                ResponsibleUser = currentUser,
                NewState = product
            };
            _eventPublisher.Publish(activationEvent);
        }

        return Converter.ToProductResponse(product);
    }
}
