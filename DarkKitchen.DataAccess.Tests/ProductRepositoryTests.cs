using DarkKitchen.Domain.Products;

namespace DarkKitchen.DataAccess.Tests;

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
        IEnumerable<Product> result = _productRepository.GetAll();

        Assert.IsNotNull(result);
        var productList = result.ToList();
        Assert.AreEqual(3, productList.Count);

        Assert.IsTrue(productList.Any(p => p.Code == "BURG01"));
        Assert.IsTrue(productList.Any(p => p.Code == "BURG02"));
        Assert.IsTrue(productList.Any(p => p.Code == "DESA01"));
    }
}
