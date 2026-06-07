using System.Security.Claims;
using DarkKitchen.Domain.Products;
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
    private ProductsController _controller = null!;
    private Mock<IProductService> _mockService = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IProductService>(MockBehavior.Strict);
        _controller = new ProductsController(_mockService.Object);
        SetCallerContext("admin@darkkitchen.com");
    }

    private void SetCallerContext(string email)
    {
        var claims = new List<Claim> { new(ClaimTypes.Email, email) };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    private static Product CreateTestProduct(string code = "BURG01", string name = "Hamburguesa Clasica")
    {
        var images = new List<ProductImage> { new("https://example.com/photo.jpg", 50000) };
        return new Product(
            code, name,
            "Hamburguesa clasica con queso cheddar",
            new ProductLine("Combo burgers"),
            new ProductCategory("Parrilla"),
            150m,
            images);
    }

    [TestMethod]
    public void GetProducts_ShouldReturnOkWithProducts()
    {
        var products = new List<Product> { CreateTestProduct("BURG01"), CreateTestProduct("BURG02", "Hamburguesa Doble Grande") };
        _mockService.Setup(s => s.GetProducts(null, null, null)).Returns(products);

        var result = _controller.GetProducts(null, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var body = (result.Value as IEnumerable<ProductResponse>)?.ToList();
        Assert.IsNotNull(body);
        Assert.AreEqual(2, body.Count);
        _mockService.VerifyAll();
    }

    [TestMethod]
    public void GetProducts_WithFilters_ShouldPassFiltersToService()
    {
        var products = new List<Product> { CreateTestProduct("BURG02", "Hamburguesa Doble Grande") };
        _mockService.Setup(s => s.GetProducts("Doble", "Combo burgers", null)).Returns(products);

        var result = _controller.GetProducts("Doble", "Combo burgers", null) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockService.VerifyAll();
    }

    [TestMethod]
    public void GetProducts_NoResults_ShouldReturn200WithEmptyList()
    {
        _mockService.Setup(s => s.GetProducts("Pizza", null, null)).Returns([]);

        var result = _controller.GetProducts("Pizza", null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockService.VerifyAll();
    }

    [TestMethod]
    public void CreateProduct_ValidRequest_Returns201()
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

        var product = CreateTestProduct("NEW01", "Nuevo Producto Test");
        _mockService.Setup(s => s.CreateProduct(request, "admin@darkkitchen.com")).Returns(product);

        var result = _controller.CreateProduct(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        _mockService.VerifyAll();
    }

    [TestMethod]
    public void UpdateProduct_ValidRequest_ReturnsOk()
    {
        var productId = Guid.NewGuid();
        var request = new ProductUpdateRequest
        {
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 200m,
            Images = [new ProductImageDto { Url = "https://example.com/new.jpg", SizeInBytes = 50000 }]
        };

        var product = CreateTestProduct("BURG01", "Hamburguesa Actualizada");
        _mockService.Setup(s => s.UpdateProduct(productId, request, "admin@darkkitchen.com")).Returns(product);

        var result = _controller.UpdateProduct(productId, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockService.VerifyAll();
    }

    [TestMethod]
    public void CreateProduct_WithNoUserClaims_ShouldUseUnknownUser()
    {
        var request = new ProductCreateRequest
        {
            Code = "U1AAA",
            Name = "Jugo de Naranja",
            Description = "D",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 10,
            Images = []
        };

        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var product = CreateTestProduct("U1AAA", "Jugo de Naranja");
        _mockService.Setup(s => s.CreateProduct(request, "Unknown")).Returns(product);

        _controller.CreateProduct(request);

        _mockService.VerifyAll();
    }

    [TestMethod]
    public void ImportProducts_ValidRequest_Returns201WithResults()
    {
        var request = new ProductImportRequest
        {
            ImporterName = "JSON Importer",
            FilePath = "/data/products.json"
        };

        var importResponse = new ProductImportResponse
        {
            TotalProcessed = 1,
            Successful = 1,
            ImportedProducts =
            [
                new ProductResponse
                {
                    Code = "IMP01", Name = "Producto Importado Test",
                    Description = "Descripcion del producto importado de prueba",
                    Line = "Desayunos", Category = "Bebidas",
                    Price = 250m, Images = ["https://img.darkkitchen.com/imported.jpg"]
                },
            ],
        };

        _mockService.Setup(s => s.ImportProducts("JSON Importer", "/data/products.json", "admin@darkkitchen.com"))
            .Returns(importResponse);

        var result = _controller.ImportProducts(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        var resultValue = result.Value as ProductImportResponse;
        Assert.IsNotNull(resultValue);
        Assert.AreEqual(1, resultValue.Successful);
        _mockService.VerifyAll();
    }

    [TestMethod]
    public void ImportProducts_WithNoUserClaims_ShouldUseUnknownUser()
    {
        var request = new ProductImportRequest
        {
            ImporterName = "JSON Importer",
            FilePath = "/data/products.json"
        };

        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        _mockService.Setup(s => s.ImportProducts("JSON Importer", "/data/products.json", "Unknown"))
            .Returns(new ProductImportResponse());

        _controller.ImportProducts(request);

        _mockService.VerifyAll();
    }

    [TestMethod]
    public void UpdateProduct_WithNoUserClaims_ShouldUseUnknownUser()
    {
        var productId = Guid.NewGuid();
        var request = new ProductUpdateRequest
        {
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Line = "Desayunos",
            Category = "Bebidas",
            Price = 200m,
            Images = [new ProductImageDto { Url = "https://example.com/new.jpg", SizeInBytes = 50000 }]
        };

        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var product = CreateTestProduct("BURG01", "Hamburguesa Actualizada");
        _mockService.Setup(s => s.UpdateProduct(productId, request, "Unknown")).Returns(product);

        _controller.UpdateProduct(productId, request);

        _mockService.VerifyAll();
    }
}
