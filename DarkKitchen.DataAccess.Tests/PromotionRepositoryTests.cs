using DarkKitchen.Domain;

namespace DarkKitchen.DataAccess.Tests;

[TestClass]
public class PromotionRepositoryTests
{
    private InMemoryPromotionRepository _promotionRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        _promotionRepository = new InMemoryPromotionRepository();
    }

    [TestMethod]
    public void Add_ValidPromotion_ShouldBeStored()
    {
        var name = "Black Friday";
        var discount = 20;
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(7);
        var products = new List<Product>();
        var promotion = new Promotion(name, discount, startDate, endDate, products);

        _promotionRepository.Add(promotion);

        var result = _promotionRepository.GetAll();
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(promotion, result.First());
    }
}
