using DarkKitchen.Domain.Products;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class ProductServiceTests
{
    private Mock<IProductRepository> _mockRepository = null!;
    private ProductService _productService = null!;
    private List<Product> _testProducts = null!;

    private List<ProductImage> _defaultImages = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IProductRepository>();
        _defaultImages = [new ProductImage("photo.jpg", 100000)];

        var lineCombo = new ProductLine("Combo burgers");
        var lineDesayunos = new ProductLine("Desayunos");
        var categoryParrilla = new ProductCategory("Parrilla");
        var categoryBebidas = new ProductCategory("Bebidas");

        _testProducts =
        [
            new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", lineCombo, categoryParrilla, 150m, _defaultImages),
            new Product("BURG02", "Hamburguesa Doble Grande", "Hamburguesa doble con queso y bacon", lineCombo, categoryParrilla, 200m, _defaultImages),
            new Product("DESA01", "Desayuno Completo Grande", "Desayuno con cafe tostadas y jugo", lineDesayunos, categoryBebidas, 120m, _defaultImages),
        ];

        _mockRepository.Setup(r => r.GetAll()).Returns(_testProducts);
        _productService = new ProductService(_mockRepository.Object);
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
        ProductCreateRequest request = new ProductCreateRequest
        {
            Code = "NEW01",
            Name = "Nuevo Producto Test",
            Description = "Descripcion del nuevo producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 100m,
            Images = [new ProductImageDto { Url = "https://example.com/photo.jpg", SizeInBytes = 50000 }],
        };

        ProductResponse result = _productService.CreateProduct(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("NEW01", result.Code);
        _mockRepository.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void UpdateProduct_ValidRequest_ShouldUpdateAndReturnResponse()
    {
        Guid productId = _testProducts[0].Id;

        ProductUpdateRequest request = new ProductUpdateRequest
        {
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 200m,
            Images = [new ProductImageDto { Url = "https://example.com/new.jpg", SizeInBytes = 50000 }],
        };

        _mockRepository.Setup(r => r.GetById(productId)).Returns(_testProducts[0]);

        ProductResponse result = _productService.UpdateProduct(productId, request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Hamburguesa Actualizada", result.Name);
        _mockRepository.Verify(r => r.Update(productId, It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void UpdateProduct_NotFound_ShouldThrow()
    {
        _mockRepository.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Product?)null);

        ProductUpdateRequest request = new ProductUpdateRequest
        {
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 200m,
            Images = [new ProductImageDto { Url = "https://example.com/new.jpg", SizeInBytes = 50000 }],
        };

        _productService.UpdateProduct(Guid.NewGuid(), request);
    }

    [TestMethod]
    public void UpdateProduct_WithIsActiveFalse_ShouldDeactivateProduct()
    {
        Guid productId = _testProducts[0].Id;

        ProductUpdateRequest request = new ProductUpdateRequest
        {
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 200m,
            Images = [new ProductImageDto { Url = "https://example.com/new.jpg", SizeInBytes = 50000 }],
            IsActive = false,
        };

        _mockRepository.Setup(r => r.GetById(productId)).Returns(_testProducts[0]);

        ProductResponse result = _productService.UpdateProduct(productId, request);

        Assert.IsFalse(result.IsActive);
    }
}
