using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class PromotionServiceTests
{
    private Mock<IProductRepository> _mockProductRepository = null!;
    private Mock<IPromotionRepository> _mockPromotionRepository = null!;
    private Mock<IDomainEventPublisher> _mockPublisher = null!;
    private PromotionService _promotionService = null!;
    private List<Product> _testProducts = null!;
    private List<Promotion> _testPromotions = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockPromotionRepository = new Mock<IPromotionRepository>(MockBehavior.Strict);
        _mockProductRepository = new Mock<IProductRepository>(MockBehavior.Strict);
        _mockPublisher = new Mock<IDomainEventPublisher>(MockBehavior.Strict);

        _promotionService = new PromotionService(
            _mockPromotionRepository.Object,
            _mockProductRepository.Object,
            _mockPublisher.Object);

        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");

        _testProducts =
        [
            new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m,
                [new ProductImage("img1.jpg", 100000)]),
            new Product("BURG02", "Hamburguesa Doble Grande", "Hamburguesa doble con queso y bacon", line, category,
                200m, [new ProductImage("img2.jpg", 150000)])
        ];

        DateTime start = DateTime.Now.AddDays(-1);
        DateTime end = DateTime.Now.AddDays(30);

        _testPromotions =
        [
            new Promotion("Black Friday", 10, start, end, [_testProducts[0]]),
            new Promotion("Semana de Turismo", 15, start, end, [_testProducts[1]])
        ];
    }

    private void SetupProductGetAll(List<Product>? products = null) =>
        _mockProductRepository.Setup(r => r.GetAll()).Returns(products ?? _testProducts);

    private void SetupPromotionGetAll(List<Promotion>? promotions = null) =>
        _mockPromotionRepository.Setup(r => r.GetAll()).Returns(promotions ?? _testPromotions);

    private void SetupPromotionGetById(Guid id, Promotion? result) =>
        _mockPromotionRepository.Setup(r => r.GetById(id)).Returns(result);

    private void SetupPromotionAdd() =>
        _mockPromotionRepository.Setup(r => r.Add(It.IsAny<Promotion>()));

    private void SetupPromotionUpdate() =>
        _mockPromotionRepository.Setup(r => r.Update(It.IsAny<Promotion>()));

    private void SetupPublishCreated() =>
        _mockPublisher.Setup(p => p.Publish(It.IsAny<EntityCreatedEvent<Promotion>>()));

    private void SetupPublishModified() =>
        _mockPublisher.Setup(p => p.Publish(It.IsAny<EntityModifiedEvent<Promotion>>()));

    [TestMethod]
    public void CreatePromotion_ValidRequest_ReturnsPromotionResponse()
    {
        SetupProductGetAll();
        SetupPromotionAdd();
        SetupPublishCreated();

        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 20,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = ["BURG01"]
        };

        var result = _promotionService.CreatePromotion(request, "admin@test.com");

        Assert.AreEqual("Promo Verano", result.Name);
        Assert.AreEqual(20, result.DiscountPercentage);
        Assert.AreEqual(1, result.Products.Count);
        Assert.IsTrue(result.Products.Contains("BURG01"));
        _mockProductRepository.VerifyAll();
        _mockPromotionRepository.VerifyAll();
        _mockPublisher.VerifyAll();
    }

    [TestMethod]
    public void CreatePromotion_ValidRequest_CallsRepositoryAdd()
    {
        SetupProductGetAll();
        SetupPromotionAdd();
        SetupPublishCreated();

        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 20,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = ["BURG01"]
        };

        _promotionService.CreatePromotion(request, "admin@test.com");

        _mockPromotionRepository.VerifyAll();
        _mockProductRepository.VerifyAll();
        _mockPublisher.VerifyAll();
    }

    [TestMethod]
    public void CreatePromotion_ValidRequest_PublishesEvent()
    {
        SetupProductGetAll();
        SetupPromotionAdd();
        _mockPublisher.Setup(p => p.Publish(It.Is<EntityCreatedEvent<Promotion>>(e =>
            e.EntityName == "Promotion" &&
            e.NewState.Name == "Promo Verano")));

        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 20,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(7),
            ProductCodes = ["BURG01"]
        };

        _promotionService.CreatePromotion(request, "admin@test.com");

        _mockProductRepository.VerifyAll();
        _mockPromotionRepository.VerifyAll();
        _mockPublisher.VerifyAll();
    }

    [TestMethod]
    public void CreatePromotion_InvalidProductCode_ThrowsArgumentException()
    {
        SetupProductGetAll();

        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 20,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = ["INVALID99"]
        };

        Assert.ThrowsException<ArgumentException>(() => _promotionService.CreatePromotion(request, "admin@test.com"));
        _mockProductRepository.VerifyAll();
    }

    [TestMethod]
    public void CreatePromotion_NoProducts_CreatesPromotionSuccessfully()
    {
        SetupProductGetAll();
        SetupPromotionAdd();
        SetupPublishCreated();

        var request = new PromotionCreateRequest
        {
            Name = "Promo Sin Productos",
            DiscountPercentage = 10,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = []
        };

        var result = _promotionService.CreatePromotion(request, "admin@test.com");

        Assert.AreEqual("Promo Sin Productos", result.Name);
        Assert.AreEqual(0, result.Products.Count);
        _mockProductRepository.VerifyAll();
        _mockPromotionRepository.VerifyAll();
        _mockPublisher.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_NoFilters_ReturnsAllPromotions()
    {
        SetupPromotionGetAll();

        var result = _promotionService.GetPromotions(null, null, null).ToList();

        Assert.AreEqual(2, result.Count);
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_FilterByDate_ReturnsMatchingPromotions()
    {
        SetupPromotionGetAll();

        var result = _promotionService.GetPromotions(DateTime.Now, null, null).ToList();

        Assert.AreEqual(2, result.Count);
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_FilterByDateOutOfRange_ReturnsEmpty()
    {
        SetupPromotionGetAll();

        var result = _promotionService.GetPromotions(new DateTime(2030, 1, 1), null, null).ToList();

        Assert.AreEqual(0, result.Count);
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_FilterByLine_ReturnsMatchingPromotions()
    {
        SetupPromotionGetAll();

        var result = _promotionService.GetPromotions(null, "Combo burgers", null).ToList();

        Assert.AreEqual(2, result.Count);
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_FilterByLine_NoMatches_ReturnsEmpty()
    {
        SetupPromotionGetAll();

        var result = _promotionService.GetPromotions(null, "Desayunos", null).ToList();

        Assert.AreEqual(0, result.Count);
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_FilterByProductCode_ReturnsMatchingPromotion()
    {
        SetupPromotionGetAll();

        var result = _promotionService.GetPromotions(null, null, "BURG01").ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result[0].Products.Contains("BURG01"));
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_FilterByProductCode_NoMatches_ReturnsEmpty()
    {
        SetupPromotionGetAll();

        var result = _promotionService.GetPromotions(null, null, "INVALID99").ToList();

        Assert.AreEqual(0, result.Count);
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_CombinedFilters_ReturnsMatchingPromotion()
    {
        SetupPromotionGetAll();

        var result = _promotionService.GetPromotions(DateTime.Now, "Combo burgers", "BURG01").ToList();

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result[0].Products.Contains("BURG01"));
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void UpdatePromotion_ValidRequest_ReturnsUpdatedResponse()
    {
        SetupPromotionGetById(_testPromotions[0].Id, _testPromotions[0]);
        SetupProductGetAll();
        SetupPromotionUpdate();
        SetupPublishModified();

        var request = new PromotionCreateRequest
        {
            Name = "Black Friday Actualizado",
            DiscountPercentage = 30,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = ["BURG02"]
        };

        var result = _promotionService.UpdatePromotion(_testPromotions[0].Id, request, "admin@test.com");

        Assert.AreEqual("Black Friday Actualizado", result.Name);
        Assert.IsTrue(result.Products.Contains("BURG02"));
        _mockPromotionRepository.VerifyAll();
        _mockProductRepository.VerifyAll();
        _mockPublisher.VerifyAll();
    }

    [TestMethod]
    public void UpdatePromotion_ValidRequest_PublishesEvent()
    {
        SetupPromotionGetById(_testPromotions[0].Id, _testPromotions[0]);
        SetupProductGetAll();
        SetupPromotionUpdate();
        _mockPublisher.Setup(p => p.Publish(It.Is<EntityModifiedEvent<Promotion>>(e =>
            e.EntityName == "Promotion" &&
            e.OldState.Name == "Black Friday" &&
            e.NewState.Name == "Black Friday Actualizado")));

        var request = new PromotionCreateRequest
        {
            Name = "Black Friday Actualizado",
            DiscountPercentage = 30,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(30),
            ProductCodes = ["BURG02"]
        };

        _promotionService.UpdatePromotion(_testPromotions[0].Id, request, "admin@test.com");

        _mockPromotionRepository.VerifyAll();
        _mockProductRepository.VerifyAll();
        _mockPublisher.VerifyAll();
    }

    [TestMethod]
    public void UpdatePromotion_NonExistentId_ThrowsKeyNotFoundException()
    {
        var unknownId = Guid.NewGuid();
        SetupPromotionGetById(unknownId, null);

        var request = new PromotionCreateRequest
        {
            Name = "Promo X",
            DiscountPercentage = 10,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = []
        };

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _promotionService.UpdatePromotion(unknownId, request, "admin@test.com"));
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void UpdatePromotion_InvalidProductCode_ThrowsArgumentException()
    {
        SetupPromotionGetById(_testPromotions[0].Id, _testPromotions[0]);
        SetupProductGetAll();

        var request = new PromotionCreateRequest
        {
            Name = "Promo Valida",
            DiscountPercentage = 10,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = ["INVALIDO99"]
        };

        Assert.ThrowsException<ArgumentException>(() =>
            _promotionService.UpdatePromotion(_testPromotions[0].Id, request, "admin@test.com"));
        _mockPromotionRepository.VerifyAll();
        _mockProductRepository.VerifyAll();
    }

    [TestMethod]
    public void UpdatePromotion_RemovingProduct_DesassociatesItCorrectly()
    {
        SetupPromotionGetById(_testPromotions[0].Id, _testPromotions[0]);
        SetupProductGetAll();
        SetupPromotionUpdate();
        SetupPublishModified();

        var request = new PromotionCreateRequest
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = []
        };

        var result = _promotionService.UpdatePromotion(_testPromotions[0].Id, request, "admin@test.com");

        Assert.AreEqual(0, result.Products.Count);
        _mockPromotionRepository.VerifyAll();
        _mockProductRepository.VerifyAll();
        _mockPublisher.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_NoDateProvided_ReturnsOnlyCurrentlyActivePromotions()
    {
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var product = new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line,
            category, 150m, [new ProductImage("burg01.jpg", 110000)]);

        var activePromo = new Promotion("Vigente", 10, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(10), [product]);
        var expiredPromo = new Promotion("Vencida", 20, new DateTime(2020, 1, 1), new DateTime(2020, 12, 31), [product]);
        SetupPromotionGetAll([activePromo, expiredPromo]);

        var result = _promotionService.GetPromotions(null, null, null).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Vigente", result[0].Name);
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_NoDateProvided_ExcludesFuturePromotions()
    {
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var product = new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line,
            category, 150m, [new ProductImage("burg01.jpg", 110000)]);

        var futurePromo = new Promotion("Futura", 15, DateTime.Now.AddDays(5), DateTime.Now.AddDays(15), [product]);
        SetupPromotionGetAll([futurePromo]);

        var result = _promotionService.GetPromotions(null, null, null).ToList();

        Assert.AreEqual(0, result.Count);
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_InactivePromotion_IsExcluded()
    {
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var product = new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line,
            category, 150m, [new ProductImage("burg01.jpg", 110000)]);

        var promo = new Promotion("Promo Inactiva", 10, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(10), [product]);
        promo.Deactivate();
        SetupPromotionGetAll([promo]);

        var result = _promotionService.GetPromotions(null, null, null).ToList();

        Assert.AreEqual(0, result.Count);
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void GetBestPromotionForProduct_WithActivePromo_ReturnsNameAndDiscount()
    {
        SetupPromotionGetAll();

        var (name, discount) = _promotionService.GetBestPromotionForProduct(_testProducts[0].Id, DateTime.Now);

        Assert.AreEqual("Black Friday", name);
        Assert.AreEqual(10m, discount);
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void GetBestPromotionForProduct_NoActivePromo_ReturnsNullAndZero()
    {
        SetupPromotionGetAll();

        var (name, discount) = _promotionService.GetBestPromotionForProduct(_testProducts[0].Id, DateTime.Now.AddDays(60));

        Assert.IsNull(name);
        Assert.AreEqual(0m, discount);
        _mockPromotionRepository.VerifyAll();
    }

    [TestMethod]
    public void GetBestPromotionForProduct_MultiplePromos_ReturnsHighestOne()
    {
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var product = new Product("SHARED01", "Producto Compartido", "Descripcion larga del producto compartido",
            line, category, 100m, [new ProductImage("shared.jpg", 120000)]);

        var promoLow = new Promotion("Promo Baja", 10, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(30), [product]);
        var promoHigh = new Promotion("Promo Alta", 40, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(30), [product]);
        SetupPromotionGetAll([promoLow, promoHigh]);

        var (name, discount) = _promotionService.GetBestPromotionForProduct(product.Id, DateTime.Now);

        Assert.AreEqual("Promo Alta", name);
        Assert.AreEqual(40m, discount);
        _mockPromotionRepository.VerifyAll();
    }
}
