using DarkKitchen.BusinessLogic;
using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class ProductServiceTests
{
    private Mock<IProductRepository> _mockRepository = null!;
    private ProductService _productService = null!;
    private List<Product> _testProducts = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IProductRepository>();

        var lineCombo = new ProductLine("Combo burgers");
        var lineDesayunos = new ProductLine("Desayunos");
        var categoryParrilla = new ProductCategory("Parrilla");
        var categoryBebidas = new ProductCategory("Bebidas");

        _testProducts =
        [
            new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", lineCombo, categoryParrilla, 150m),
            new Product("BURG02", "Hamburguesa Doble Grande", "Hamburguesa doble con queso y bacon", lineCombo, categoryParrilla, 200m),
            new Product("DESA01", "Desayuno Completo Grande", "Desayuno con cafe tostadas y jugo", lineDesayunos, categoryBebidas, 120m),
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
}
