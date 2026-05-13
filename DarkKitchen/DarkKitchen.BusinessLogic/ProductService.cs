using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Products;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;
using DarkKitchen.Plugin.Contracts;

namespace DarkKitchen.BusinessLogic;

public class ProductService(
    IProductRepository productRepository,
    IDomainEventPublisher eventPublisher,
    IEnumerable<IProductImporter> importers)
    : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IDomainEventPublisher _eventPublisher = eventPublisher;
    private readonly IEnumerable<IProductImporter> _importers = importers;

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

        ProductLine line = GetOrCreateLine(request.Line);
        ProductCategory category = GetOrCreateCategory(request.Category);
        var images = request.Images
            .Select(i => new ProductImage(i.Url, i.SizeInBytes))
            .ToList();

        var product = new Product(request.Code, request.Name, request.Description, line, category, request.Price,
            images);

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

    public ProductImportResponse ImportProducts(string importerName, string filePath, string currentUser)
    {
        IProductImporter importer = _importers.FirstOrDefault(i =>
                                        i.Name.Equals(importerName, StringComparison.OrdinalIgnoreCase))
                                    ?? throw new ArgumentException($"Importer '{importerName}' not found.");

        IEnumerable<ProductImportDto> importedDtos = importer.ImportProducts(filePath);
        var response = new ProductImportResponse { TotalProcessed = importedDtos.Count() };

        // Optimización: Cargamos datos existentes una sola vez para evitar N+1 queries
        var existingCodes = _productRepository.GetAll().Select(p => p.Code).ToHashSet();
        var existingLines = _productRepository.GetAllLines()
            .GroupBy(l => l.Name.Trim().ToLower())
            .ToDictionary(g => g.Key, g => g.First());
        var existingCategories = _productRepository.GetAllCategories()
            .GroupBy(c => c.Name.Trim().ToLower())
            .ToDictionary(g => g.Key, g => g.First());

        foreach(ProductImportDto dto in importedDtos)
        {
            try
            {
                if(existingCodes.Contains(dto.Code!))
                {
                    throw new ArgumentException($"Product with code {dto.Code} already exists.");
                }

                // Obtener o crear línea (usando caché en memoria)
                var lineNameKey = dto.LineName!.Trim().ToLower();

                if(!existingLines.TryGetValue(lineNameKey, out ProductLine? line))
                {
                    line = new ProductLine(dto.LineName!.Trim());
                    existingLines[lineNameKey] = line;
                }

                // Obtener o crear categoría (usando caché en memoria)
                var categoryNameKey = dto.CategoryName!.Trim().ToLower();

                if(!existingCategories.TryGetValue(categoryNameKey, out ProductCategory? category))
                {
                    category = new ProductCategory(dto.CategoryName!.Trim());
                    existingCategories[categoryNameKey] = category;
                }

                List<ProductImage> images = dto.Images?
                    .Select(i => new ProductImage(i.Url!, i.SizeInBytes))
                    .ToList() ?? [];

                var product = new Product(
                    dto.Code!,
                    dto.Name!,
                    dto.Description!,
                    line,
                    category,
                    dto.Price,
                    images);

                _productRepository.Add(product);
                existingCodes.Add(dto.Code!); // Agregamos al caché para evitar duplicados en el mismo archivo

                var domainEvent = new EntityCreatedEvent<Product>
                {
                    EntityId = product.Id,
                    EntityName = nameof(Product),
                    ResponsibleUser = currentUser,
                    NewState = product
                };
                _eventPublisher.Publish(domainEvent);

                response.ImportedProducts.Add(Converter.ToProductResponse(product));
                response.Successful++;
            }
            catch(Exception ex)
            {
                response.Failed++;
                response.Errors.Add($"Code {dto.Code ?? "Unknown"}: {ex.Message}");
            }
        }

        return response;
    }

    private ProductLine GetOrCreateLine(string name)
    {
        return _productRepository.GetAllLines()
                   .FirstOrDefault(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
               ?? new ProductLine(name);
    }

    private ProductCategory GetOrCreateCategory(string name)
    {
        return _productRepository.GetAllCategories()
                   .FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
               ?? new ProductCategory(name);
    }
}
