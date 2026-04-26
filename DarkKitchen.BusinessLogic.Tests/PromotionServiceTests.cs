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

    [TestMethod]
    public void CreatePromotion_ValidRequest_CallsRepositoryAdd()
    {
        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 20,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = ["BURG01"]
        };

        _promotionService.CreatePromotion(request);

        _mockPromotionRepository.Verify(r => r.Add(It.IsAny<Promotion>()), Times.Once);
    }

    [TestMethod]
    public void CreatePromotion_InvalidProductCode_ThrowsArgumentException()
    {
        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 20,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = ["INVALID99"]
        };

        Assert.ThrowsException<ArgumentException>(() => _promotionService.CreatePromotion(request));
    }

    [TestMethod]
    public void CreatePromotion_NoProducts_CreatesPromotionSuccessfully()
    {
        var request = new PromotionCreateRequest
        {
            Name = "Promo Sin Productos",
            DiscountPercentage = 10,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = []
        };

        PromotionCreateResponse result = _promotionService.CreatePromotion(request);

        Assert.AreEqual("Promo Sin Productos", result.Name);
        Assert.AreEqual(0, result.Products.Count);
    }

    [TestMethod]
    public void GetPromotions_NoFilters_ReturnsAllPromotions()
    {
        var result = _promotionService.GetPromotions(null, null, null).ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetPromotions_FilterByDate_ReturnsMatchingPromotions()
    {
        var result = _promotionService.GetPromotions(new DateTime(2025, 6, 1), null, null).ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetPromotions_FilterByDateOutOfRange_ReturnsEmpty()
    {
        var result = _promotionService.GetPromotions(new DateTime(2030, 1, 1), null, null).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetPromotions_FilterByLine_ReturnsMatchingPromotions()
    {
        var result = _promotionService.GetPromotions(null, "Combo burgers", null).ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetPromotions_FilterByLine_NoMatches_ReturnsEmpty()
    {
        var result = _promotionService.GetPromotions(null, "Desayunos", null).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetPromotions_FilterByProductCode_ReturnsMatchingPromotion()
    {
        var result = _promotionService.GetPromotions(null, null, "BURG01").ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result[0].Products.Contains("BURG01"));
    }

    [TestMethod]
    public void GetPromotions_FilterByProductCode_NoMatches_ReturnsEmpty()
    {
        var result = _promotionService.GetPromotions(null, null, "INVALID99").ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetPromotions_CombinedFilters_ReturnsMatchingPromotion()
    {
        var result = _promotionService.GetPromotions(new DateTime(2025, 6, 1), "Combo burgers", "BURG01").ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result[0].Products.Contains("BURG01"));
    }

    [TestMethod]
    public void UpdatePromotion_ValidRequest_ReturnsUpdatedResponse()
    {
        Guid promoId = _testPromotions[0].Id;
        var request = new PromotionCreateRequest
        {
            Name = "Black Friday Actualizado",
            DiscountPercentage = 30,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = ["BURG02"]
        };

        PromotionCreateResponse result = _promotionService.UpdatePromotion(promoId, request);

        Assert.AreEqual("Black Friday Actualizado", result.Name);
        Assert.AreEqual(30, result.DiscountPercentage);
        Assert.IsTrue(result.Products.Contains("BURG02"));
    }

    [TestMethod]
    public void UpdatePromotion_NonExistentId_ThrowsKeyNotFoundException()
    {
        var request = new PromotionCreateRequest
        {
            Name = "Promo X",
            DiscountPercentage = 10,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = []
        };

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _promotionService.UpdatePromotion(Guid.NewGuid(), request));
    }

    [TestMethod]
    public void UpdatePromotion_InvalidProductCode_ThrowsArgumentException()
    {
        Guid promoId = _testPromotions[0].Id;
        var request = new PromotionCreateRequest
        {
            Name = "Promo Valida",
            DiscountPercentage = 10,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = ["INVALIDO99"]
        };

        Assert.ThrowsException<ArgumentException>(() =>
            _promotionService.UpdatePromotion(promoId, request));
    }

    [TestMethod]
    public void UpdatePromotion_RemovingProduct_DesassociatesItCorrectly()
    {
        Guid promoId = _testPromotions[0].Id;
        var request = new PromotionCreateRequest
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = []
        };

        PromotionCreateResponse result = _promotionService.UpdatePromotion(promoId, request);

        Assert.AreEqual(0, result.Products.Count);
    }
}
