using DarkKitchen.Domain;

namespace DarkKitchen.Tests;

[TestClass]
public class ProductTests
{
    private ProductLine defaultLine = null!;
    private ProductCategory defaultCategory = null!;

    [TestInitialize]
    public void Setup()
    {
        defaultLine = new ProductLine("Combo burgers");
        defaultCategory = new ProductCategory("Parrilla");
    }

    [TestMethod]
    public void CreateProduct_WithValidData_ShouldCreateSuccessfully()
    {
        var product = new Product(
            "ABC12",
            "Hamburguesa",
            "Hamburguesa con queso cheddar",
            defaultLine,
            defaultCategory,
            150.50m);

        Assert.AreEqual("ABC12", product.Code);
        Assert.AreEqual("Hamburguesa", product.Name);
        Assert.AreEqual("Hamburguesa con queso cheddar", product.Description);
        Assert.AreEqual(defaultLine, product.Line);
        Assert.AreEqual(defaultCategory, product.Category);
        Assert.AreEqual(150.50m, product.Price);
        Assert.IsTrue(product.IsActive);
    }
}
