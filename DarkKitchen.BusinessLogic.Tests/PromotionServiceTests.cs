using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class PromotionServiceTests
{
    private Mock<IProductRepository> _mockProductRepository = null!;
    private Mock<IPromotionRepository> _mockPromotionRepository = null!;
    private PromotionService _promotionService = null!;
    private List<Product> _testProducts = null!;
    private List<Promotion> _testPromotions = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockPromotionRepository = new Mock<IPromotionRepository>();
        _mockProductRepository = new Mock<IProductRepository>();

        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");

        _testProducts =
        [
            new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m),
            new Product("BURG02", "Hamburguesa Doble Grande", "Hamburguesa doble con queso y bacon", line, category,
                200m)
        ];

        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2025, 12, 31);

        _testPromotions =
        [
            new Promotion("Black Friday", 10, start, end, [_testProducts[0]]),
            new Promotion("Semana de Turismo", 15, start, end, [_testProducts[1]])
        ];

        _mockProductRepository.Setup(r => r.GetAll()).Returns(_testProducts);
        _mockPromotionRepository.Setup(r => r.GetAll()).Returns(_testPromotions);

        _promotionService = new PromotionService(
            _mockPromotionRepository.Object,
            _mockProductRepository.Object
        );
    }

    [TestMethod]
    public void CreatePromotion_ValidRequest_ReturnsPromotionResponse()
    {
        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 20,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = ["BURG01"]
        };

        PromotionCreateResponse result = _promotionService.CreatePromotion(request);

        Assert.AreEqual("Promo Verano", result.Name);
        Assert.AreEqual(20, result.DiscountPercentage);
        Assert.AreEqual(1, result.Products.Count);
        Assert.IsTrue(result.Products.Contains("BURG01"));
    }
}
