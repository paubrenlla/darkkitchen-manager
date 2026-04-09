using DarkKitchen.DataAccess;
using DarkKitchen.Domain;

namespace DarkKitchen.Tests;

[TestClass]
public class ProductRepositoryTests
{
    private InMemoryProductRepository _productRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        _productRepository = new InMemoryProductRepository();
    }

    [TestMethod]
    public void GetAll_ReturnsSeededProducts()
    {
        var result = _productRepository.GetAll();

        Assert.IsNotNull(result);
        var productList = result.ToList();
        Assert.AreEqual(3, productList.Count);

        Assert.IsTrue(productList.Any(p => p.Code == "BURG01"));
        Assert.IsTrue(productList.Any(p => p.Code == "BURG02"));
        Assert.IsTrue(productList.Any(p => p.Code == "DESA01"));
    }
}
