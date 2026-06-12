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
        _mockRepository = new Mock<IProductRepository>(MockBehavior.Strict);
        _mockEventPublisher = new Mock<IDomainEventPublisher>(MockBehavior.Strict);
        _defaultImages = [new ProductImage("photo.jpg", 100000)];

        var lineCombo = new ProductLine("Combo burgers");
        var lineDesayunos = new ProductLine("Desayunos");
        var categoryParrilla = new ProductCategory("Parrilla");
        var categoryBebidas = new ProductCategory("Bebidas");

        _testProducts =
        [
            new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", lineCombo, categoryParrilla, 150m, _defaultImages),
            new Product("BURG02", "Hamburguesa Doble Grande", "Hamburguesa doble con queso y bacon", lineCombo, categoryParrilla, 200m, _defaultImages),
            new Product("DESA01", "Desayuno Completo Grande", "Desayuno con cafe tostadas y jugo", lineDesayunos, categoryBebidas, 120m, _defaultImages)
        ];

        _productService = new ProductService(_mockRepository.Object, _mockEventPublisher.Object, []);
    }

    private void SetupRepositoryGetAll(List<Product>? products = null) =>
        _mockRepository.Setup(r => r.GetAll()).Returns(products ?? _testProducts);

    private void SetupRepositoryGetAllLines() =>
        _mockRepository.Setup(r => r.GetAllLines()).Returns(_testProducts.Select(p => p.Line).DistinctBy(l => l.Name));

    private void SetupRepositoryGetAllCategories() =>
        _mockRepository.Setup(r => r.GetAllCategories()).Returns(_testProducts.Select(p => p.Category).DistinctBy(c => c.Name));

    private void SetupRepositoryGetById(Guid id, Product? product) =>
        _mockRepository.Setup(r => r.GetById(id)).Returns(product);

    private void SetupRepositoryAdd() =>
        _mockRepository.Setup(r => r.Add(It.IsAny<Product>()));

    private void SetupRepositoryUpdate() =>
        _mockRepository.Setup(r => r.Update(It.IsAny<Guid>(), It.IsAny<Product>()));

    private void SetupPublishCreated() =>
        _mockEventPublisher.Setup(p => p.Publish(It.IsAny<EntityCreatedEvent<Product>>()));

    private void SetupPublishModified() =>
        _mockEventPublisher.Setup(p => p.Publish(It.IsAny<EntityModifiedEvent<Product>>()));

    private void SetupPublishDeactivated() =>
        _mockEventPublisher.Setup(p => p.Publish(It.IsAny<EntityDeactivatedEvent<Product>>()));

    private void SetupPublishActivated() =>
        _mockEventPublisher.Setup(p => p.Publish(It.IsAny<EntityActivatedEvent<Product>>()));

    [TestMethod]
    public void GetProducts_NoFilters_ShouldReturnAllProducts()
    {
        SetupRepositoryGetAll();

        var result = _productService.GetProducts(null, null, null).ToList();

        Assert.AreEqual(3, result.Count);
        _mockRepository.VerifyAll();
    }

    [TestMethod]
    public void GetProducts_FilterByName_ShouldReturnMatchingProducts()
    {
        SetupRepositoryGetAll();

        var result = _productService.GetProducts("Doble", null, null).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG02", result[0].Code);
        _mockRepository.VerifyAll();
    }

    [TestMethod]
    public void GetProducts_FilterByLine_ShouldReturnMatchingProducts()
    {
        SetupRepositoryGetAll();

        var result = _productService.GetProducts(null, "Combo burgers", null).ToList();

        Assert.AreEqual(2, result.Count);
        _mockRepository.VerifyAll();
    }

    [TestMethod]
    public void GetProducts_FilterByCategory_ShouldReturnMatchingProducts()
    {
        SetupRepositoryGetAll();

        var result = _productService.GetProducts(null, null, "Bebidas").ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("DESA01", result[0].Code);
        _mockRepository.VerifyAll();
    }

    [TestMethod]
    public void GetProducts_FilterByNameAndLine_ShouldCombineFilters()
    {
        SetupRepositoryGetAll();

        var result = _productService.GetProducts("Hamburguesa", "Combo burgers", null).ToList();

        Assert.AreEqual(2, result.Count);
        _mockRepository.VerifyAll();
    }

    [TestMethod]
    public void GetProducts_NoMatches_ShouldReturnEmptyList()
    {
        SetupRepositoryGetAll();

        var result = _productService.GetProducts("Pizza", null, null).ToList();

        Assert.AreEqual(0, result.Count);
        _mockRepository.VerifyAll();
    }

    [TestMethod]
    public void GetProducts_FilterByNamePartialMatch_ShouldReturnMatchingProducts()
    {
        SetupRepositoryGetAll();

        var result = _productService.GetProducts("Grande", null, null).ToList();

        Assert.AreEqual(2, result.Count);
        _mockRepository.VerifyAll();
    }

    [TestMethod]
    public void CreateProduct_ValidRequest_ShouldAddAndReturnResponse()
    {
        SetupRepositoryGetAll([]);
        SetupRepositoryGetAllLines();
        SetupRepositoryGetAllCategories();
        SetupRepositoryAdd();
        SetupPublishCreated();

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

        var result = _productService.CreateProduct(request, "admin@darkkitchen.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("NEW01", result.Code);
        _mockRepository.VerifyAll();
        _mockEventPublisher.Verify(p => p.Publish(It.Is<EntityCreatedEvent<Product>>(e =>
            e.EntityName == "Product" &&
            e.ResponsibleUser == "admin@darkkitchen.com" &&
            e.NewState.Code == "NEW01")), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_DuplicatedCode_ShouldThrow()
    {
        SetupRepositoryGetAll();

        var request = new ProductCreateRequest
        {
            Code = "BURG01",
            Name = "Valid Product Name",
            Description = "This is a valid long description",
            Line = "Combo burgers",
            Category = "Parrilla",
            Price = 100,
            Images = [new ProductImageDto { Url = "valid.jpg", SizeInBytes = 1000 }]
        };

        _productService.CreateProduct(request, "admin@darkkitchen.com");

        _mockRepository.VerifyAll();
    }

    [TestMethod]
    public void UpdateProduct_ValidRequest_ShouldUpdateAndReturnResponse()
    {
        var productId = _testProducts[0].Id;
        SetupRepositoryGetById(productId, _testProducts[0]);
        SetupRepositoryGetAllLines();
        SetupRepositoryGetAllCategories();
        SetupRepositoryUpdate();
        SetupPublishModified();

        var request = new ProductUpdateRequest
        {
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 200m,
            Images = [new ProductImageDto { Url = "https://example.com/new.jpg", SizeInBytes = 50000 }]
        };

        var result = _productService.UpdateProduct(productId, request, "admin@darkkitchen.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("Hamburguesa Actualizada", result.Name);
        _mockRepository.VerifyAll();
        _mockEventPublisher.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void UpdateProduct_NotFound_ShouldThrow()
    {
        SetupRepositoryGetById(It.IsAny<Guid>(), null);

        var request = new ProductUpdateRequest
        {
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 200m,
            Images = [new ProductImageDto { Url = "https://example.com/new.jpg", SizeInBytes = 50000 }]
        };

        var unknownId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetById(unknownId)).Returns((Product?)null);
        _productService.UpdateProduct(unknownId, request, "admin@darkkitchen.com");
        _mockRepository.VerifyAll();
    }

    [TestMethod]
    public void UpdateProduct_WithIsActiveFalse_ShouldDeactivateProduct()
    {
        var productId = _testProducts[0].Id;
        SetupRepositoryGetById(productId, _testProducts[0]);
        SetupRepositoryGetAllLines();
        SetupRepositoryGetAllCategories();
        SetupRepositoryUpdate();
        SetupPublishModified();
        SetupPublishDeactivated();

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

        var result = _productService.UpdateProduct(productId, request, "admin@darkkitchen.com");

        Assert.IsFalse(result.IsActive);
        _mockRepository.VerifyAll();
        _mockEventPublisher.Verify(p => p.Publish(It.Is<EntityDeactivatedEvent<Product>>(e =>
            e.EntityName == "Product" &&
            e.ResponsibleUser == "admin@darkkitchen.com" &&
            e.EntityId == productId)), Times.Once);
    }

    [TestMethod]
    public void UpdateProduct_WithIsActiveTrue_ShouldActivateProduct()
    {
        var productId = _testProducts[0].Id;
        _testProducts[0].Deactivate();
        SetupRepositoryGetById(productId, _testProducts[0]);
        SetupRepositoryGetAllLines();
        SetupRepositoryGetAllCategories();
        SetupRepositoryUpdate();
        SetupPublishModified();
        SetupPublishActivated();

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

        var result = _productService.UpdateProduct(productId, request, "admin@darkkitchen.com");

        Assert.IsTrue(result.IsActive);
        _mockRepository.VerifyAll();
        _mockEventPublisher.Verify(p => p.Publish(It.Is<EntityActivatedEvent<Product>>(e =>
            e.EntityName == "Product" &&
            e.ResponsibleUser == "admin@darkkitchen.com" &&
            e.EntityId == productId)), Times.Once);
    }

    [TestMethod]
    public void UpdateProduct_ValidRequest_ShouldPublishEntityModifiedEvent()
    {
        var productId = _testProducts[0].Id;
        var expectedOldPrice = _testProducts[0].Price;
        SetupRepositoryGetById(productId, _testProducts[0]);
        SetupRepositoryGetAllLines();
        SetupRepositoryGetAllCategories();
        SetupRepositoryUpdate();
        SetupPublishModified();

        var request = new ProductUpdateRequest
        {
            Name = "New Valid Name",
            Description = _testProducts[0].Description,
            Line = "Combo burgers",
            Category = "Parrilla",
            Price = 150m,
            Images = [new ProductImageDto { Url = "photo.jpg", SizeInBytes = 100000 }]
        };

        _productService.UpdateProduct(productId, request, "admin@darkkitchen.com");

        _mockRepository.VerifyAll();
        _mockEventPublisher.Verify(p => p.Publish(It.Is<EntityModifiedEvent<Product>>(e =>
            e.EntityId == productId &&
            e.ResponsibleUser == "admin@darkkitchen.com" &&
            e.OldState.Price == expectedOldPrice &&
            e.NewState.Price == 150m &&
            !ReferenceEquals(e.OldState, e.NewState))), Times.Once);
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
        SetupRepositoryGetAll();
        SetupRepositoryGetAllLines();
        SetupRepositoryGetAllCategories();

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

        var mockImporter = new Mock<IProductImporter>(MockBehavior.Strict);
        mockImporter.Setup(i => i.Name).Returns("Test Importer");
        mockImporter.Setup(i => i.ImportProducts(It.IsAny<string>())).Returns([duplicateDto]);

        var service = new ProductService(_mockRepository.Object, _mockEventPublisher.Object, [mockImporter.Object]);

        var result = service.ImportProducts("Test Importer", "file.json", "admin@darkkitchen.com");

        Assert.AreEqual(0, result.Successful);
        Assert.AreEqual(1, result.Failed);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].Contains("already exists"));
        _mockRepository.VerifyAll();
        mockImporter.VerifyAll();
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

        var mockImporter = new Mock<IProductImporter>(MockBehavior.Strict);
        mockImporter.Setup(i => i.Name).Returns("Test Importer");
        mockImporter.Setup(i => i.ImportProducts(It.IsAny<string>())).Returns([importDto]);

        var emptyRepo = new Mock<IProductRepository>(MockBehavior.Strict);
        emptyRepo.Setup(r => r.GetAll()).Returns([]);
        emptyRepo.Setup(r => r.GetAllLines()).Returns([]);
        emptyRepo.Setup(r => r.GetAllCategories()).Returns([]);
        emptyRepo.Setup(r => r.Add(It.IsAny<Product>()));

        SetupPublishCreated();

        var service = new ProductService(emptyRepo.Object, _mockEventPublisher.Object, [mockImporter.Object]);

        var result = service.ImportProducts("Test Importer", "file.json", "admin@darkkitchen.com");

        Assert.AreEqual(1, result.Successful);
        Assert.AreEqual(0, result.Failed);
        Assert.AreEqual("IMP01", result.ImportedProducts[0].Code);
        emptyRepo.VerifyAll();
        mockImporter.VerifyAll();
        _mockEventPublisher.Verify(p => p.Publish(It.Is<EntityCreatedEvent<Product>>(e =>
            e.EntityName == nameof(Product) &&
            e.ResponsibleUser == "admin@darkkitchen.com")), Times.Once);
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
            Price = -10m,
            Images = []
        };

        var mockImporter = new Mock<IProductImporter>(MockBehavior.Strict);
        mockImporter.Setup(i => i.Name).Returns("Test Importer");
        mockImporter.Setup(i => i.ImportProducts(It.IsAny<string>())).Returns([validDto, invalidDto]);

        var emptyRepo = new Mock<IProductRepository>(MockBehavior.Strict);
        emptyRepo.Setup(r => r.GetAll()).Returns([]);
        emptyRepo.Setup(r => r.GetAllLines()).Returns([]);
        emptyRepo.Setup(r => r.GetAllCategories()).Returns([]);
        emptyRepo.Setup(r => r.Add(It.IsAny<Product>()));

        SetupPublishCreated();

        var service = new ProductService(emptyRepo.Object, _mockEventPublisher.Object, [mockImporter.Object]);

        var result = service.ImportProducts("Test Importer", "file.json", "admin@darkkitchen.com");

        Assert.AreEqual(2, result.TotalProcessed);
        Assert.AreEqual(1, result.Successful);
        Assert.AreEqual(1, result.Failed);
        Assert.AreEqual(1, result.ImportedProducts.Count);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].Contains("INVALID01"));
        emptyRepo.VerifyAll();
        mockImporter.VerifyAll();
        _mockEventPublisher.VerifyAll();
    }

    [TestMethod]
    public void ImportProducts_ExistingLineAndCategory_ShouldReuseInstances()
    {
        var existingLine = _testProducts[0].Line;
        var existingCategory = _testProducts[0].Category;

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

        var mockImporter = new Mock<IProductImporter>(MockBehavior.Strict);
        mockImporter.Setup(i => i.Name).Returns("Test Importer");
        mockImporter.Setup(i => i.ImportProducts(It.IsAny<string>())).Returns([importDto]);

        SetupRepositoryGetAll([]);
        _mockRepository.Setup(r => r.GetAllLines()).Returns([existingLine]);
        _mockRepository.Setup(r => r.GetAllCategories()).Returns([existingCategory]);
        SetupRepositoryAdd();
        SetupPublishCreated();

        var service = new ProductService(_mockRepository.Object, _mockEventPublisher.Object, [mockImporter.Object]);

        var result = service.ImportProducts("Test Importer", "file.json", "admin@darkkitchen.com");

        Assert.AreEqual(1, result.Successful);
        _mockRepository.VerifyAll();
        mockImporter.VerifyAll();
        _mockEventPublisher.VerifyAll();
        _mockRepository.Verify(r => r.Add(It.Is<Product>(p =>
            p.Line.Id == existingLine.Id &&
            p.Category.Id == existingCategory.Id)), Times.Once);
    }
}
