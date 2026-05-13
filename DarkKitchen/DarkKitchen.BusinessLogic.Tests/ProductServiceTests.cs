using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Products;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;
using DarkKitchen.Plugin.Contracts;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class ProductServiceTests
{
    private List<ProductImage> _defaultImages = null!;
    private Mock<IDomainEventPublisher> _mockEventPublisher = null!;
    private Mock<IProductRepository> _mockRepository = null!;
    private ProductService _productService = null!;
    private List<Product> _testProducts = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IProductRepository>();
        _mockEventPublisher = new Mock<IDomainEventPublisher>();
        _defaultImages = [new ProductImage("photo.jpg", 100000)];

        var lineCombo = new ProductLine("Combo burgers");
        var lineDesayunos = new ProductLine("Desayunos");
        var categoryParrilla = new ProductCategory("Parrilla");
        var categoryBebidas = new ProductCategory("Bebidas");

        _testProducts =
        [
            new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", lineCombo,
                categoryParrilla, 150m, _defaultImages),
            new Product("BURG02", "Hamburguesa Doble Grande", "Hamburguesa doble con queso y bacon", lineCombo,
                categoryParrilla, 200m, _defaultImages),
            new Product("DESA01", "Desayuno Completo Grande", "Desayuno con cafe tostadas y jugo", lineDesayunos,
                categoryBebidas, 120m, _defaultImages)
        ];

        _mockRepository.Setup(r => r.GetAll()).Returns(_testProducts);
        _mockRepository.Setup(r => r.GetAllLines()).Returns(_testProducts.Select(p => p.Line).DistinctBy(l => l.Name));
        _mockRepository.Setup(r => r.GetAllCategories()).Returns(_testProducts.Select(p => p.Category).DistinctBy(c => c.Name));
        _productService = new ProductService(_mockRepository.Object, _mockEventPublisher.Object, []);
    }

    [TestMethod]
    public void GetProducts_NoFilters_ShouldReturnAllProducts()
    {
        var result = _productService.GetProducts(null, null, null).ToList();

        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void GetProducts_FilterByName_ShouldReturnMatchingProducts()
    {
        var result = _productService.GetProducts("Doble", null, null).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG02", result[0].Code);
    }

    [TestMethod]
    public void GetProducts_FilterByLine_ShouldReturnMatchingProducts()
    {
        var result = _productService.GetProducts(null, "Combo burgers", null).ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetProducts_FilterByCategory_ShouldReturnMatchingProducts()
    {
        var result = _productService.GetProducts(null, null, "Bebidas").ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("DESA01", result[0].Code);
    }

    [TestMethod]
    public void GetProducts_FilterByNameAndLine_ShouldCombineFilters()
    {
        var result = _productService.GetProducts("Hamburguesa", "Combo burgers", null).ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetProducts_NoMatches_ShouldReturnEmptyList()
    {
        var result = _productService.GetProducts("Pizza", null, null).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetProducts_FilterByNamePartialMatch_ShouldReturnMatchingProducts()
    {
        var result = _productService.GetProducts("Grande", null, null).ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void CreateProduct_ValidRequest_ShouldAddAndReturnResponse()
    {
        var request = new ProductCreateRequest
        {
            Code = "NEW01",
            Name = "Nuevo Producto Test",
            Description = "Descripcion del nuevo producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 100m,
            Images = [new ProductImageDto { Url = "https://example.com/photo.jpg", SizeInBytes = 50000 }]
        };

        ProductResponse result = _productService.CreateProduct(request, "admin@darkkitchen.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("NEW01", result.Code);
        _mockRepository.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
        _mockEventPublisher.Verify(p => p.Publish(It.Is<EntityCreatedEvent<Product>>(e =>
            e.EntityName == "Product" && e.ResponsibleUser == "admin@darkkitchen.com" && e.NewState.Code == "NEW01")), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_DuplicatedCode_ShouldThrow()
    {
        var request = new ProductCreateRequest
        {
            Code = "BURG01", // This code already exists in _testProducts
            Name = "Valid Product Name",
            Description = "This is a valid long description",
            Line = "Combo burgers",
            Category = "Parrilla",
            Price = 100,
            Images = [new ProductImageDto { Url = "valid.jpg", SizeInBytes = 1000 }]
        };

        _productService.CreateProduct(request, "admin@darkkitchen.com");
    }

    [TestMethod]
    public void UpdateProduct_ValidRequest_ShouldUpdateAndReturnResponse()
    {
        Guid productId = _testProducts[0].Id;

        var request = new ProductUpdateRequest
        {
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 200m,
            Images = [new ProductImageDto { Url = "https://example.com/new.jpg", SizeInBytes = 50000 }]
        };

        _mockRepository.Setup(r => r.GetById(productId)).Returns(_testProducts[0]);

        ProductResponse result = _productService.UpdateProduct(productId, request, "admin@darkkitchen.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("Hamburguesa Actualizada", result.Name);
        _mockRepository.Verify(r => r.Update(productId, It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void UpdateProduct_NotFound_ShouldThrow()
    {
        _mockRepository.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Product?)null);

        var request = new ProductUpdateRequest
        {
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 200m,
            Images = [new ProductImageDto { Url = "https://example.com/new.jpg", SizeInBytes = 50000 }]
        };

        _productService.UpdateProduct(Guid.NewGuid(), request, "admin@darkkitchen.com");
    }

    [TestMethod]
    public void UpdateProduct_WithIsActiveFalse_ShouldDeactivateProduct()
    {
        Guid productId = _testProducts[0].Id;

        var request = new ProductUpdateRequest
        {
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 200m,
            Images = [new ProductImageDto { Url = "https://example.com/new.jpg", SizeInBytes = 50000 }],
            IsActive = false
        };

        _mockRepository.Setup(r => r.GetById(productId)).Returns(_testProducts[0]);

        ProductResponse result = _productService.UpdateProduct(productId, request, "admin@darkkitchen.com");

        Assert.IsFalse(result.IsActive);
        _mockEventPublisher.Verify(p => p.Publish(It.Is<EntityDeactivatedEvent<Product>>(e =>
            e.EntityName == "Product" && e.ResponsibleUser == "admin@darkkitchen.com" && e.EntityId == productId)), Times.Once);
    }

    [TestMethod]
    public void UpdateProduct_WithIsActiveTrue_ShouldActivateProduct()
    {
        Guid productId = _testProducts[0].Id;

        // Deactivate it first so we can test the activation branch
        _testProducts[0].Deactivate();

        var request = new ProductUpdateRequest
        {
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 200m,
            Images = [new ProductImageDto { Url = "https://example.com/new.jpg", SizeInBytes = 50000 }],
            IsActive = true
        };

        _mockRepository.Setup(r => r.GetById(productId)).Returns(_testProducts[0]);

        ProductResponse result = _productService.UpdateProduct(productId, request, "admin@darkkitchen.com");

        Assert.IsTrue(result.IsActive);
        _mockEventPublisher.Verify(p => p.Publish(It.Is<EntityActivatedEvent<Product>>(e =>
            e.EntityName == "Product" && e.ResponsibleUser == "admin@darkkitchen.com" && e.EntityId == productId)), Times.Once);
    }

    [TestMethod]
    public void UpdateProduct_ValidRequest_ShouldPublishEntityModifiedEvent()
    {
        Guid productId = _testProducts[0].Id;
        Product existingProduct = _testProducts[0];

        var expectedOldPrice = existingProduct.Price;

        var updateRequest = new ProductUpdateRequest
        {
            Name = "New Valid Name",
            Description = existingProduct.Description,
            Line = "Combo burgers",
            Category = "Parrilla",
            Price = 150m,
            Images = [new ProductImageDto { Url = "photo.jpg", SizeInBytes = 100000 }]
        };

        var currentUser = "admin@darkkitchen.com";

        _mockRepository.Setup(r => r.GetById(productId)).Returns(existingProduct);

        _productService.UpdateProduct(productId, updateRequest, currentUser);

        _mockEventPublisher.Verify(
            p => p.Publish(It.Is<EntityModifiedEvent<Product>>(e =>
                e.EntityId == productId &&
                e.ResponsibleUser == currentUser &&
                e.OldState.Price == expectedOldPrice &&
                e.NewState.Price == 150m &&
                !ReferenceEquals(e.OldState, e.NewState))),
            Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ImportProducts_ImporterNotFound_ShouldThrow()
    {
        _productService.ImportProducts("NonExistentImporter", "file.json", "admin@darkkitchen.com");
    }

    [TestMethod]
    public void ImportProducts_DuplicateCode_ShouldNotThrowButReportError()
    {
        var duplicateDto = new ProductImportDto
        {
            Code = "BURG01",
            Name = "Producto Duplicado Test",
            Description = "Descripcion de producto duplicado test",
            LineName = "Combo burgers",
            CategoryName = "Parrilla",
            Price = 100m,
            Images = [new ImageImportDto { Url = "https://img.darkkitchen.com/photo.jpg", SizeInBytes = 10000 }]
        };

        var mockImporter = new Mock<IProductImporter>();
        mockImporter.Setup(i => i.Name).Returns("Test Importer");
        mockImporter.Setup(i => i.ImportProducts(It.IsAny<string>())).Returns([duplicateDto]);

        var service = new ProductService(_mockRepository.Object, _mockEventPublisher.Object, [mockImporter.Object]);
        var result = service.ImportProducts("Test Importer", "file.json", "admin@darkkitchen.com");

        Assert.AreEqual(0, result.Successful);
        Assert.AreEqual(1, result.Failed);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].Contains("already exists"));
    }

    [TestMethod]
    public void ImportProducts_ValidImport_ShouldCreateAndReturnProducts()
    {
        var importDto = new ProductImportDto
        {
            Code = "IMP01",
            Name = "Producto Importado Test",
            Description = "Descripcion de producto importado test",
            LineName = "Desayunos",
            CategoryName = "Bebidas",
            Price = 250m,
            Images = [new ImageImportDto { Url = "https://img.darkkitchen.com/imported.jpg", SizeInBytes = 15000 }]
        };

        var mockImporter = new Mock<IProductImporter>();
        mockImporter.Setup(i => i.Name).Returns("Test Importer");
        mockImporter.Setup(i => i.ImportProducts(It.IsAny<string>())).Returns([importDto]);

        var emptyRepo = new Mock<IProductRepository>();
        emptyRepo.Setup(r => r.GetAll()).Returns([]);

        var service = new ProductService(emptyRepo.Object, _mockEventPublisher.Object, [mockImporter.Object]);

        var result = service.ImportProducts("Test Importer", "file.json", "admin@darkkitchen.com");

        Assert.AreEqual(1, result.Successful);
        Assert.AreEqual(0, result.Failed);
        Assert.AreEqual("IMP01", result.ImportedProducts[0].Code);
        Assert.AreEqual("Producto Importado Test", result.ImportedProducts[0].Name);
        Assert.AreEqual(250m, result.ImportedProducts[0].Price);
        emptyRepo.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
        _mockEventPublisher.Verify(p => p.Publish(It.Is<EntityCreatedEvent<Product>>(e =>
            e.EntityName == nameof(Product) && e.ResponsibleUser == "admin@darkkitchen.com")), Times.Once);
    }

    [TestMethod]
    public void ImportProducts_PartialSuccess_ShouldReportBoth()
    {
        var validDto = new ProductImportDto
        {
            Code = "VALID01",
            Name = "Producto Valido Largo",
            Description = "Esta es una descripcion lo suficientemente larga para que pase el dominio.",
            LineName = "Combo burgers",
            CategoryName = "Parrilla",
            Price = 100m,
            Images = [new ImageImportDto { Url = "https://img.darkkitchen.com/ok.jpg", SizeInBytes = 100 }]
        };
        var invalidDto = new ProductImportDto
        {
            Code = "INVALID01",
            Name = "Invalido",
            Description = "Desc",
            LineName = "Combo burgers",
            CategoryName = "Parrilla",
            Price = -10m, // Precio inválido
            Images = [] // Sin imágenes
        };

        var mockImporter = new Mock<IProductImporter>();
        mockImporter.Setup(i => i.Name).Returns("Test Importer");
        mockImporter.Setup(i => i.ImportProducts(It.IsAny<string>())).Returns([validDto, invalidDto]);

        var emptyRepo = new Mock<IProductRepository>();
        emptyRepo.Setup(r => r.GetAll()).Returns([]);
        emptyRepo.Setup(r => r.GetAllLines()).Returns([]);
        emptyRepo.Setup(r => r.GetAllCategories()).Returns([]);

        var service = new ProductService(emptyRepo.Object, _mockEventPublisher.Object, [mockImporter.Object]);

        var result = service.ImportProducts("Test Importer", "file.json", "admin@darkkitchen.com");

        Assert.AreEqual(2, result.TotalProcessed);
        Assert.AreEqual(1, result.Successful);
        Assert.AreEqual(1, result.Failed);
        Assert.AreEqual(1, result.ImportedProducts.Count);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].Contains("INVALID01"));

        // Verificamos la optimización: 1 sola llamada para 2 productos
        emptyRepo.Verify(r => r.GetAll(), Times.Once);
        emptyRepo.Verify(r => r.GetAllLines(), Times.Once);
        emptyRepo.Verify(r => r.GetAllCategories(), Times.Once);
    }

    [TestMethod]
    public void ImportProducts_ExistingLineAndCategory_ShouldReuseInstances()
    {
        var existingLine = _testProducts[0].Line; // "Combo burgers"
        var existingCategory = _testProducts[0].Category; // "Parrilla"

        var importDto = new ProductImportDto
        {
            Code = "IMP02",
            Name = "Producto Con Linea Existente",
            Description = "Descripcion del producto con linea existente",
            LineName = "Combo burgers",
            CategoryName = "Parrilla",
            Price = 100m,
            Images = [new ImageImportDto { Url = "https://img.darkkitchen.com/photo.jpg", SizeInBytes = 10000 }]
        };

        var mockImporter = new Mock<IProductImporter>();
        mockImporter.Setup(i => i.Name).Returns("Test Importer");
        mockImporter.Setup(i => i.ImportProducts(It.IsAny<string>())).Returns([importDto]);

        _mockRepository.Setup(r => r.GetAll()).Returns([]);
        _mockRepository.Setup(r => r.GetAllLines()).Returns([existingLine]);
        _mockRepository.Setup(r => r.GetAllCategories()).Returns([existingCategory]);

        var service = new ProductService(_mockRepository.Object, _mockEventPublisher.Object, [mockImporter.Object]);

        var result = service.ImportProducts("Test Importer", "file.json", "admin@darkkitchen.com");

        Assert.AreEqual(1, result.Successful);
        _mockRepository.Verify(r => r.Add(It.Is<Product>(p =>
            p.Line.Id == existingLine.Id &&
            p.Category.Id == existingCategory.Id)), Times.Once);

        _mockRepository.Verify(r => r.GetAll(), Times.Once);
        _mockRepository.Verify(r => r.GetAllLines(), Times.Once);
        _mockRepository.Verify(r => r.GetAllCategories(), Times.Once);
    }
}
