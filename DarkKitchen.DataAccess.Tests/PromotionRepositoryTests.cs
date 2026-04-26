using DarkKitchen.Domain;
using DarkKitchen.Domain.Products;

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
        DateTime startDate = DateTime.Today;
        DateTime endDate = DateTime.Today.AddDays(7);
        var products = new List<Product>();
        var promotion = new Promotion(name, discount, startDate, endDate, products);

        _promotionRepository.Add(promotion);

        IEnumerable<Promotion> result = _promotionRepository.GetAll();
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(promotion, result.First());
    }
}
