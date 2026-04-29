using DarkKitchen.Domain.Products;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class ProductLineTests
{
    [TestMethod]
    public void CreateProductLine_WithValidName_ShouldCreateSuccessfully()
    {
        var line = new ProductLine("Combo burgers");

        Assert.AreEqual("Combo burgers", line.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProductLine_WithEmptyName_ShouldThrowException()
    {
        new ProductLine(string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProductLine_WithNullName_ShouldThrowException()
    {
        new ProductLine(null);
    }
}
