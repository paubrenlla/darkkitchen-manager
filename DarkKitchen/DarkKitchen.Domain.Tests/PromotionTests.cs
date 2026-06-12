using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class PromotionTests
{
    private static List<Product> CreateDefaultProducts()
    {
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        return new List<Product>
        {
            new("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m, [new ProductImage("img.jpg", 100000)])
        };
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePromotion_WithEmptyName_ShouldThrowException()
    {
        var discount = 10;
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now.AddDays(1);
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var products = new List<Product>
        {
            new("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m, [new ProductImage("img.jpg", 100000)])
        };

        new Promotion(null, discount, startDate, endDate, products);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePromotion_WithShortName_ShouldThrowException()
    {
        var name = "Ab";
        var discount = 10;
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now.AddDays(1);
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var products = new List<Product>
        {
            new("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m, [new ProductImage("img.jpg", 100000)])
        };

        new Promotion(name, discount, startDate, endDate, products);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePromotion_WithLongName_ShouldThrowException()
    {
        var name = new string('A', 51);
        var discount = 10;
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now.AddDays(1);
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var products = new List<Product>
        {
            new("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m, [new ProductImage("img.jpg", 100000)])
        };

        new Promotion(name, discount, startDate, endDate, products);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePromotion_WithInvalidDiscount_ShouldThrowException()
    {
        var name = "Valid Name";
        var discount = 0;
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now.AddDays(1);
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var products = new List<Product>
        {
            new("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m, [new ProductImage("img.jpg", 100000)])
        };

        new Promotion(name, discount, startDate, endDate, products);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePromotion_WithInvalidDates_ShouldThrowException()
    {
        var name = "Valid Name";
        var discount = 10;
        DateTime startDate = DateTime.Today;
        DateTime endDate = DateTime.Today.AddDays(-1);
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var products = new List<Product>
        {
            new("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m, [new ProductImage("img.jpg", 100000)])
        };

        new Promotion(name, discount, startDate, endDate, products);
    }

    [TestMethod]
    public void CreatePromotion_WithSameDates_ShouldSucceed()
    {
        var name = "Valid Name";
        var discount = 10;
        DateTime startDate = DateTime.Today;
        DateTime endDate = DateTime.Today;
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var products = new List<Product>
        {
            new("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m, [new ProductImage("img.jpg", 100000)])
        };

        var promotion = new Promotion(name, discount, startDate, endDate, products);

        Assert.IsNotNull(promotion);
    }

    [TestMethod]
    public void CreatePromotion_WithValidData()
    {
        var name = "Valid Name";
        var discount = 10;
        DateTime startDate = DateTime.Today;
        DateTime endDate = DateTime.Today.AddDays(1);
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var products = new List<Product>
        {
            new("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m, [new ProductImage("img.jpg", 100000)])
        };

        var promotion = new Promotion(name, discount, startDate, endDate, products);

        Assert.IsNotNull(promotion);
    }

    [TestMethod]
    public void UpdatePromotion_WithValidData_ShouldUpdateProperties()
    {
        var products = CreateDefaultProducts();
        var promotion = new Promotion("Nombre Viejo", 10, DateTime.Today, DateTime.Today.AddDays(5), products);

        var newProducts = CreateDefaultProducts();
        promotion.Update("Nombre Nuevo", 25, DateTime.Today, DateTime.Today.AddDays(10), newProducts);

        Assert.AreEqual("Nombre Nuevo", promotion.Name);
        Assert.AreEqual(25, promotion.DiscountPercentage);
        Assert.AreEqual(1, promotion.Products.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdatePromotion_WithInvalidName_ShouldThrow()
    {
        var products = CreateDefaultProducts();
        var promotion = new Promotion("Nombre Valido", 10, DateTime.Today, DateTime.Today.AddDays(5), products);
        promotion.Update("Ab", 10, DateTime.Today, DateTime.Today.AddDays(5), products);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdatePromotion_WithInvalidDiscount_ShouldThrow()
    {
        var products = CreateDefaultProducts();
        var promotion = new Promotion("Nombre Valido", 10, DateTime.Today, DateTime.Today.AddDays(5), products);
        promotion.Update("Nombre Valido", 0, DateTime.Today, DateTime.Today.AddDays(5), products);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdatePromotion_WithInvalidDates_ShouldThrow()
    {
        var products = CreateDefaultProducts();
        var promotion = new Promotion("Nombre Valido", 10, DateTime.Today, DateTime.Today.AddDays(5), products);
        promotion.Update("Nombre Valido", 10, DateTime.Today.AddDays(5), DateTime.Today, products);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdatePromotion_WithNoProducts_ShouldThrowException()
    {
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var products = new List<Product>
        {
            new("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m, [new ProductImage("img.jpg", 100000)])
        };
        var promotion = new Promotion(
            "Promo",
            10,
            DateTime.Today,
            DateTime.Today.AddDays(5),
            products);

        promotion.Update(
            "Promo",
            10,
            DateTime.Today,
            DateTime.Today.AddDays(5),
            []);
    }

    [TestMethod]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var products = CreateDefaultProducts();
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), products);

        promotion.Deactivate();

        Assert.IsFalse(promotion.IsActive);
    }

    [TestMethod]
    public void IsVigente_DateWithinRange_ReturnsTrue()
    {
        var products = CreateDefaultProducts();
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), products);

        Assert.IsTrue(promotion.IsVigente(DateTime.Today.AddDays(2)));
    }

    [TestMethod]
    public void IsVigente_DateBeforeStart_ReturnsFalse()
    {
        var products = CreateDefaultProducts();
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), products);

        Assert.IsFalse(promotion.IsVigente(DateTime.Today.AddDays(-1)));
    }

    [TestMethod]
    public void IsVigente_DateAfterEnd_ReturnsFalse()
    {
        var products = CreateDefaultProducts();
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), products);

        Assert.IsFalse(promotion.IsVigente(DateTime.Today.AddDays(10)));
    }

    [TestMethod]
    public void IsVigente_WhenInactive_ReturnsFalse()
    {
        var products = CreateDefaultProducts();
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), products);
        promotion.Deactivate();

        Assert.IsFalse(promotion.IsVigente(DateTime.Today.AddDays(2)));
    }

    [TestMethod]
    public void IsVigente_OnStartDate_ReturnsTrue()
    {
        var products = CreateDefaultProducts();
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), products);

        Assert.IsTrue(promotion.IsVigente(DateTime.Today));
    }

    [TestMethod]
    public void IsVigente_OnEndDate_ReturnsTrue()
    {
        var products = CreateDefaultProducts();
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), products);

        Assert.IsTrue(promotion.IsVigente(DateTime.Today.AddDays(5)));
    }

    [TestMethod]
    public void Clone_ActivePromotion_ShouldReturnActiveClone()
    {
        var products = CreateDefaultProducts();
        var promotion = new Promotion("Promo", 10, DateTime.Today, DateTime.Today.AddDays(5), products);

        var clone = promotion.Clone();

        Assert.AreEqual(promotion.Id, clone.Id);
        Assert.AreEqual(promotion.Name, clone.Name);
        Assert.IsTrue(clone.IsActive);
    }

    [TestMethod]
    public void Clone_InactivePromotion_ShouldReturnInactiveClone()
    {
        var products = CreateDefaultProducts();
        var promotion = new Promotion("Promo", 10, DateTime.Today, DateTime.Today.AddDays(5), products);
        promotion.Deactivate();

        var clone = promotion.Clone();

        Assert.IsFalse(clone.IsActive);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePromotion_WithNoProducts_ShouldThrowException()
    {
        new Promotion(
            "Promo",
            10,
            DateTime.Today,
            DateTime.Today.AddDays(5),
            []);
    }
}
