using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class PromotionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePromotion_WithEmptyName_ShouldThrowException()
    {
        var discount = 10;
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now.AddDays(1);
        var products = new List<Product>();

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
        var products = new List<Product>();

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
        var products = new List<Product>();

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
        var products = new List<Product>();

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
        var products = new List<Product>();

        new Promotion(name, discount, startDate, endDate, products);
    }

    [TestMethod]
    public void CreatePromotion_WithSameDates_ShouldSucceed()
    {
        var name = "Valid Name";
        var discount = 10;
        DateTime startDate = DateTime.Today;
        DateTime endDate = DateTime.Today;
        var products = new List<Product>();

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
        var products = new List<Product>();

        var promotion = new Promotion(name, discount, startDate, endDate, products);

        Assert.IsNotNull(promotion);
    }

    [TestMethod]
    public void UpdatePromotion_WithValidData_ShouldUpdateProperties()
    {
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var products = new List<Product>
        {
            new("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m, [new ProductImage("img.jpg", 100000)])
        };
        var promotion = new Promotion("Nombre Viejo", 10, DateTime.Today, DateTime.Today.AddDays(5), []);

        promotion.Update("Nombre Nuevo", 25, DateTime.Today, DateTime.Today.AddDays(10), products);

        Assert.AreEqual("Nombre Nuevo", promotion.Name);
        Assert.AreEqual(25, promotion.DiscountPercentage);
        Assert.AreEqual(1, promotion.Products.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdatePromotion_WithInvalidName_ShouldThrow()
    {
        var promotion = new Promotion("Nombre Valido", 10, DateTime.Today, DateTime.Today.AddDays(5), []);
        promotion.Update("Ab", 10, DateTime.Today, DateTime.Today.AddDays(5), []);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdatePromotion_WithInvalidDiscount_ShouldThrow()
    {
        var promotion = new Promotion("Nombre Valido", 10, DateTime.Today, DateTime.Today.AddDays(5), []);
        promotion.Update("Nombre Valido", 0, DateTime.Today, DateTime.Today.AddDays(5), []);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdatePromotion_WithInvalidDates_ShouldThrow()
    {
        var promotion = new Promotion("Nombre Valido", 10, DateTime.Today, DateTime.Today.AddDays(5), []);
        promotion.Update("Nombre Valido", 10, DateTime.Today.AddDays(5), DateTime.Today, []);
    }

    [TestMethod]
    public void UpdatePromotion_WithEmptyProducts_ShouldDesassociateAll()
    {
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var product = new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line,
            category, 150m, [new ProductImage("img.jpg", 100000)]);
        var promotion = new Promotion("Promo", 10, DateTime.Today, DateTime.Today.AddDays(5), [product]);

        promotion.Update("Promo", 10, DateTime.Today, DateTime.Today.AddDays(5), []);

        Assert.AreEqual(0, promotion.Products.Count);
    }

    [TestMethod]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), []);

        promotion.Deactivate();

        Assert.IsFalse(promotion.IsActive);
    }

    [TestMethod]
    public void IsVigente_DateWithinRange_ReturnsTrue()
    {
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), []);

        Assert.IsTrue(promotion.IsVigente(DateTime.Today.AddDays(2)));
    }

    [TestMethod]
    public void IsVigente_DateBeforeStart_ReturnsFalse()
    {
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), []);

        Assert.IsFalse(promotion.IsVigente(DateTime.Today.AddDays(-1)));
    }

    [TestMethod]
    public void IsVigente_DateAfterEnd_ReturnsFalse()
    {
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), []);

        Assert.IsFalse(promotion.IsVigente(DateTime.Today.AddDays(10)));
    }

    [TestMethod]
    public void IsVigente_WhenInactive_ReturnsFalse()
    {
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), []);
        promotion.Deactivate();

        Assert.IsFalse(promotion.IsVigente(DateTime.Today.AddDays(2)));
    }

    [TestMethod]
    public void IsVigente_OnStartDate_ReturnsTrue()
    {
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), []);

        Assert.IsTrue(promotion.IsVigente(DateTime.Today));
    }

    [TestMethod]
    public void IsVigente_OnEndDate_ReturnsTrue()
    {
        var promotion = new Promotion("Promo Valida", 10, DateTime.Today, DateTime.Today.AddDays(5), []);

        Assert.IsTrue(promotion.IsVigente(DateTime.Today.AddDays(5)));
    }
}
