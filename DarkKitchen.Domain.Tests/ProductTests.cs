using DarkKitchen.Domain;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class ProductTests
{
    private ProductLine defaultLine = null!;
    private ProductCategory defaultCategory = null!;
    private List<ProductImage> defaultImages = null!;

    [TestInitialize]
    public void Setup()
    {
        defaultLine = new ProductLine("Combo burgers");
        defaultCategory = new ProductCategory("Parrilla");
        defaultImages = [new ProductImage("photo1.jpg", 100000)];
    }

    [TestMethod]
    public void CreateProduct_WithValidData_ShouldCreateSuccessfully()
    {
        Product product = new Product(
            "ABC12",
            "Hamburguesa",
            "Hamburguesa con queso cheddar",
            defaultLine,
            defaultCategory,
            150.50m,
            defaultImages);

        Assert.AreEqual("ABC12", product.Code);
        Assert.AreEqual("Hamburguesa", product.Name);
        Assert.AreEqual("Hamburguesa con queso cheddar", product.Description);
        Assert.AreEqual(defaultLine, product.Line);
        Assert.AreEqual(defaultCategory, product.Category);
        Assert.AreEqual(150.50m, product.Price);
        Assert.IsTrue(product.IsActive);
        Assert.AreEqual(1, product.Images.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WithCodeTooShort_ShouldThrowException()
    {
        new Product("AB", "Hamburguesa", "Hamburguesa con queso cheddar", defaultLine, defaultCategory, 100m, defaultImages);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WithCodeTooLong_ShouldThrowException()
    {
        new Product("ABCDEFGHIJKLMNOPQRSTU", "Hamburguesa", "Hamburguesa con queso cheddar", defaultLine, defaultCategory, 100m, defaultImages);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WithNonAlphanumericCode_ShouldThrowException()
    {
        new Product("ABC@!", "Hamburguesa", "Hamburguesa con queso cheddar", defaultLine, defaultCategory, 100m, defaultImages);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WithNameTooShort_ShouldThrowException()
    {
        new Product("ABC12", "Corto", "Hamburguesa con queso cheddar", defaultLine, defaultCategory, 100m, defaultImages);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WithNameTooLong_ShouldThrowException()
    {
        new Product("ABC12", new string('A', 51), "Hamburguesa con queso cheddar", defaultLine, defaultCategory, 100m, defaultImages);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WithDescriptionTooShort_ShouldThrowException()
    {
        new Product("ABC12", "Hamburguesa", "Corta", defaultLine, defaultCategory, 100m, defaultImages);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WithDescriptionTooLong_ShouldThrowException()
    {
        new Product("ABC12", "Hamburguesa", new string('A', 501), defaultLine, defaultCategory, 100m, defaultImages);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WithNegativePrice_ShouldThrowException()
    {
        new Product("ABC12", "Hamburguesa", "Hamburguesa con queso cheddar", defaultLine, defaultCategory, -10m, defaultImages);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WithZeroPrice_ShouldThrowException()
    {
        new Product("ABC12", "Hamburguesa", "Hamburguesa con queso cheddar", defaultLine, defaultCategory, 0m, defaultImages);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WithNullLine_ShouldThrowException()
    {
        new Product("ABC12", "Hamburguesa", "Hamburguesa con queso cheddar", null!, defaultCategory, 100m, defaultImages);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WithNullCategory_ShouldThrowException()
    {
        new Product("ABC12", "Hamburguesa", "Hamburguesa con queso cheddar", defaultLine, null!, 100m, defaultImages);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WithNoImages_ShouldThrowException()
    {
        new Product("ABC12", "Hamburguesa", "Hamburguesa con queso cheddar", defaultLine, defaultCategory, 100m, []);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateProduct_WithMoreThanThreeImages_ShouldThrowException()
    {
        List<ProductImage> images =
        [
            new ProductImage("a.jpg", 1000),
            new ProductImage("b.jpg", 1000),
            new ProductImage("c.jpg", 1000),
            new ProductImage("d.jpg", 1000),
        ];

        new Product("ABC12", "Hamburguesa", "Hamburguesa con queso cheddar", defaultLine, defaultCategory, 100m, images);
    }

    [TestMethod]
    public void CreateProduct_WithThreeImages_ShouldSucceed()
    {
        List<ProductImage> images =
        [
            new ProductImage("a.jpg", 1000),
            new ProductImage("b.jpg", 1000),
            new ProductImage("c.jpg", 1000),
        ];

        Product product = new Product("ABC12", "Hamburguesa", "Hamburguesa con queso cheddar", defaultLine, defaultCategory, 100m, images);

        Assert.AreEqual(3, product.Images.Count);
    }

    [TestMethod]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        Product product = new Product("ABC12", "Hamburguesa", "Hamburguesa con queso cheddar", defaultLine, defaultCategory, 100m, defaultImages);

        product.Deactivate();

        Assert.IsFalse(product.IsActive);
    }

    [TestMethod]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        Product product = new Product("ABC12", "Hamburguesa", "Hamburguesa con queso cheddar", defaultLine, defaultCategory, 100m, defaultImages);
        product.Deactivate();

        product.Activate();

        Assert.IsTrue(product.IsActive);
    }
}
