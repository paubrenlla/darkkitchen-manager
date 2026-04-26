using DarkKitchen.Domain.Products;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class ProductCategoryTests
{
    [TestMethod]
    public void CreateProductCategory_WithValidName_ShouldCreateSuccessfully()
    {
        var category = new ProductCategory("Parrilla");

        Assert.AreEqual("Parrilla", category.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProductCategory_WithEmptyName_ShouldThrowException()
    {
        new ProductCategory(string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProductCategory_WithNullName_ShouldThrowException()
    {
        new ProductCategory(null);
    }
}
