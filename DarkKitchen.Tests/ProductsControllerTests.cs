using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.Tests;

[TestClass]
public class ProductsControllerTests
{
    private Mock<IProductService> _mockService = null!;
    private ProductsController _controller = null!;
    private List<Product> _testProducts = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IProductService>();

        var lineCombo = new ProductLine("Combo burgers");
        var categoryParrilla = new ProductCategory("Parrilla");

        _testProducts =
        [
            new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", lineCombo, categoryParrilla, 150m),
            new Product("BURG02", "Hamburguesa Doble Grande", "Hamburguesa doble con queso y bacon", lineCombo, categoryParrilla, 200m),
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

    var products = (result.Value as IEnumerable<object>)?.ToList();
    Assert.IsNotNull(products);
    Assert.AreEqual(2, products.Count);
}

    [TestMethod]
    public void GetProducts_WithFilters_ShouldPassFiltersToService()
    {
        _mockService.Setup(s => s.GetProducts("Doble", "Combo burgers", null)).Returns(new List<Product> { _testProducts[1] });

        var result = _controller.GetProducts("Doble", "Combo burgers", null) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockService.Verify(s => s.GetProducts("Doble", "Combo burgers", null), Times.Once);
    }

    [TestMethod]
    public void GetProducts_NoResults_ShouldReturnOkWithEmptyList()
    {
        _mockService.Setup(s => s.GetProducts("Pizza", null, null)).Returns(new List<Product>());

        var result = _controller.GetProducts("Pizza", null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }
}
