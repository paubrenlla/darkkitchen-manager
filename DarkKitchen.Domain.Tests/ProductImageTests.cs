using DarkKitchen.Domain;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class ProductImageTests
{
    [TestMethod]
    public void CreateImage_WithValidData_ShouldSucceed()
    {
        ProductImage image = new ProductImage("photo.jpg", 100000);

        Assert.AreEqual("photo.jpg", image.FileName);
        Assert.AreEqual(100000, image.SizeInBytes);
    }
}
