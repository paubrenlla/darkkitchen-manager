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
    private List<ProductResponse> _testProducts = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IProductService>();

        _testProducts =
        [
            new ProductResponse
            {
                Code = "BURG01",
                Name = "Hamburguesa Clasica",
                Description = "Hamburguesa clasica con queso cheddar",
                Price = 150m,
                Line = "Combo burgers",
                Category = "Parrilla",
                Images = []
            },
            new ProductResponse
            {
                Code = "BURG02",
                Name = "Hamburguesa Doble Grande",
                Description = "Hamburguesa doble con queso y bacon",
                Price = 200m,
                Line = "Combo burgers",
                Category = "Parrilla",
                Images = []
            },
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
    public void GetProducts_NoResults_ShouldReturn204WithEmptyList()
    {
        _mockService.Setup(s => s.GetProducts("Pizza", null, null)).Returns([]);

        var result = _controller.GetProducts("Pizza", null, null) as NoContentResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(204, result.StatusCode);
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

        var response = new ProductResponse
        {
            Code = "NEW01",
            Name = "Nuevo Producto Test",
            Description = "Descripcion del nuevo producto de prueba",
            Price = 100m,
            Line = "Desayunos",
            Category = "Bebidas",
            Images = ["https://example.com/photo.jpg"],
            IsActive = true
        };

        _mockService.Setup(s => s.CreateProduct(request, It.IsAny<string>())).Returns(response);

        var claims = new List<System.Security.Claims.Claim> { new(System.Security.Claims.ClaimTypes.Email, "admin@darkkitchen.com") };
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "Test");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = _controller.CreateProduct(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
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

        var response = new ProductResponse
        {
            Code = "BURG01",
            Name = "Hamburguesa Actualizada",
            Description = "Descripcion actualizada del producto de prueba",
            Price = 200m,
            Line = "Desayunos",
            Category = "Bebidas",
            Images = ["https://example.com/new.jpg"],
            IsActive = true
        };

        _mockService.Setup(s => s.UpdateProduct(productId, request, It.IsAny<string>())).Returns(response);

        var claims = new List<System.Security.Claims.Claim> { new(System.Security.Claims.ClaimTypes.Email, "admin@darkkitchen.com") };
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "Test");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = _controller.UpdateProduct(productId, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void CreateProduct_WithNoUserClaims_ShouldUseUnknownUser()
    {
        var request = new ProductCreateRequest { Code = "U1", Name = "N", Description = "D", Line = "Desayunos", Category = "Bebidas", Price = 10, Images = [] };
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        _controller.CreateProduct(request);

        _mockService.Verify(s => s.CreateProduct(request, "Unknown"), Times.Once);
    }

    [TestMethod]
    public void ImportProducts_ValidRequest_Returns201WithResults()
    {
        var request = new ProductImportRequest
        {
            ImporterName = "JSON Importer",
            FilePath = "/data/products.json"
        };

        var importedProducts = new List<ProductResponse>
        {
            new()
            {
                Code = "IMP01",
                Name = "Producto Importado Test",
                Description = "Descripcion de producto importado test",
                Price = 250m,
                Line = "Desayunos",
                Category = "Bebidas",
                Images = ["https://img.darkkitchen.com/imported.jpg"],
                IsActive = true
            }
        };

        _mockService
            .Setup(s => s.ImportProducts("JSON Importer", "/data/products.json", "admin@darkkitchen.com"))
            .Returns(importedProducts);

        var claims = new List<System.Security.Claims.Claim> { new(System.Security.Claims.ClaimTypes.Email, "admin@darkkitchen.com") };
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "Test");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = _controller.ImportProducts(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        var resultValue = result.Value as List<ProductResponse>;
        Assert.IsNotNull(resultValue);
        Assert.AreEqual(1, resultValue.Count);
        Assert.AreEqual("IMP01", resultValue[0].Code);
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

        _controller.ImportProducts(request);

        _mockService.Verify(s => s.ImportProducts("JSON Importer", "/data/products.json", "Unknown"), Times.Once);
    }
}
