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
}
