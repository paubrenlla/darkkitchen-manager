using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
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
            new ProductResponse { Code = "BURG01", Name = "Hamburguesa Clasica", Description = "Hamburguesa clasica con queso cheddar", Price = 150m, Line = "Combo burgers", Category = "Parrilla" },
            new ProductResponse { Code = "BURG02", Name = "Hamburguesa Doble Grande", Description = "Hamburguesa doble con queso y bacon", Price = 200m, Line = "Combo burgers", Category = "Parrilla" },
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
        _mockService.Setup(s => s.GetProducts("Pizza", null, null)).Returns(new List<ProductResponse>());

        var result = _controller.GetProducts("Pizza", null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }
}
