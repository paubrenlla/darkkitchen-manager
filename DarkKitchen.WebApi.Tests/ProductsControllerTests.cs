using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class ProductsControllerTests
{
    private Mock<IProductService> _mockService = null!;
    private ProductsController _controller = null!;
    private List<ProductResponse> _testProducts = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IProductService>();

        _testProducts =
        [
            new ProductResponse { Code = "BURG01", Name = "Hamburguesa Clasica", Description = "Hamburguesa clasica con queso cheddar", Price = 150m, Line = "Combo burgers", Category = "Parrilla", Images = [] },
            new ProductResponse { Code = "BURG02", Name = "Hamburguesa Doble Grande", Description = "Hamburguesa doble con queso y bacon", Price = 200m, Line = "Combo burgers", Category = "Parrilla", Images = [] },
        ];

        _controller = new ProductsController(_mockService.Object);
    }

    [TestMethod]
    public void GetProducts_ShouldReturnOkWithProducts()
    {
        _mockService.Setup(s => s.GetProducts(null, null, null)).Returns(_testProducts);

        var result = _controller.GetProducts(null, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var products = (result.Value as IEnumerable<ProductResponse>)?.ToList();
        Assert.IsNotNull(products);
        Assert.AreEqual(2, products.Count);
    }

    [TestMethod]
    public void GetProducts_WithFilters_ShouldPassFiltersToService()
    {
        _mockService.Setup(s => s.GetProducts("Doble", "Combo burgers", null)).Returns([_testProducts[1]]);

        var result = _controller.GetProducts("Doble", "Combo burgers", null) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockService.Verify(s => s.GetProducts("Doble", "Combo burgers", null), Times.Once);
    }

    [TestMethod]
    public void GetProducts_NoResults_ShouldReturnOkWithEmptyList()
    {
        _mockService.Setup(s => s.GetProducts("Pizza", null, null)).Returns([]);

        var result = _controller.GetProducts("Pizza", null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void CreateProduct_ValidRequest_Returns201()
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

        ProductResponse response = new ProductResponse
        {
            Code = "NEW01",
            Name = "Nuevo Producto Test",
            Description = "Descripcion del nuevo producto de prueba",
            Price = 100m,
            Line = "Desayunos",
            Category = "Bebidas",
            Images = ["https://example.com/photo.jpg"],
            IsActive = true,
        };

        _mockService.Setup(s => s.CreateProduct(request)).Returns(response);

        ObjectResult? result = _controller.CreateProduct(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
    }

    [TestMethod]
    public void CreateProduct_InvalidData_ReturnsBadRequest()
    {
        ProductCreateRequest request = new ProductCreateRequest
        {
            Code = "AB",
            Name = "Corto",
            Description = "Corta",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 100m,
            Images = [new ProductImageDto { Url = "https://example.com/photo.jpg", SizeInBytes = 50000 }],
        };

        _mockService.Setup(s => s.CreateProduct(request))
            .Throws(new ArgumentException("Code must be between 5 and 20 alphanumeric characters."));

        BadRequestObjectResult? result = _controller.CreateProduct(request) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void UpdateProduct_ValidRequest_ReturnsOk()
    {
        Guid productId = Guid.NewGuid();

        ProductUpdateRequest request = new ProductUpdateRequest
        {
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 200m,
            Images = [new ProductImageDto { Url = "https://example.com/new.jpg", SizeInBytes = 50000 }],
        };

        ProductResponse response = new ProductResponse
        {
            Code = "BURG01",
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Price = 200m,
            Line = "Desayunos",
            Category = "Bebidas",
            Images = ["https://example.com/new.jpg"],
            IsActive = true,
        };

        _mockService.Setup(s => s.UpdateProduct(productId, request)).Returns(response);

        OkObjectResult? result = _controller.UpdateProduct(productId, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void UpdateProduct_NotFound_ReturnsNotFound()
    {
        Guid productId = Guid.NewGuid();

        ProductUpdateRequest request = new ProductUpdateRequest
        {
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 200m,
            Images = [new ProductImageDto { Url = "https://example.com/new.jpg", SizeInBytes = 50000 }],
        };

        _mockService.Setup(s => s.UpdateProduct(productId, request))
            .Throws(new KeyNotFoundException("Producto no encontrado."));

        NotFoundObjectResult? result = _controller.UpdateProduct(productId, request) as NotFoundObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
    }
}
