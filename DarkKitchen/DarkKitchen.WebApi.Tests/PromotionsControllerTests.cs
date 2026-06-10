using System.Security.Claims;
using DarkKitchen.Domain.Promotions;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class PromotionsControllerTests
{
    private PromotionsController _controller = null!;
    private Mock<IPromotionService> _mockService = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IPromotionService>(MockBehavior.Strict);
        _controller = new PromotionsController(_mockService.Object);
        SetCallerContext("admin@test.com");
    }

    private void SetCallerContext(string email)
    {
        var claims = new List<Claim> { new(ClaimTypes.Email, email) };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    private static Promotion CreateTestPromotion(string name = "Black Friday", int discount = 10)
    {
        return new Promotion(
            name,
            discount,
            new DateTime(2026, 1, 1),
            new DateTime(2026, 12, 31),
            []);
    }

    [TestMethod]
    public void GetPromotions_NoFilters_ShouldReturnOkWithPromotions()
    {
        var promotions = new List<Promotion>
        {
            CreateTestPromotion("Black Friday", 10),
            CreateTestPromotion("Semana de Turismo", 15),
        };
        _mockService.Setup(s => s.GetPromotions(null, null, null)).Returns(promotions);

        var result = _controller.GetPromotions(null, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var body = (result.Value as IEnumerable<PromotionCreateResponse>)?.ToList();
        Assert.IsNotNull(body);
        Assert.AreEqual(2, body.Count);
        _mockService.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_WithFilters_ShouldPassFiltersToService()
    {
        var promotions = new List<Promotion> { CreateTestPromotion() };
        _mockService.Setup(s => s.GetPromotions(null, "Combo burgers", "BURG01")).Returns(promotions);

        var result = _controller.GetPromotions(null, "Combo burgers", "BURG01") as OkObjectResult;

        Assert.IsNotNull(result);
        _mockService.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_NoResults_ShouldReturnOkWithEmptyList()
    {
        _mockService.Setup(s => s.GetPromotions(null, "Desayunos", null)).Returns([]);

        var result = _controller.GetPromotions(null, "Desayunos", null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockService.VerifyAll();
    }

    [TestMethod]
    public void CreatePromotion_ValidRequest_ShouldReturn201()
    {
        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 20,
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            ProductCodes = []
        };

        var promotion = CreateTestPromotion("Promo Verano", 20);
        _mockService.Setup(s => s.CreatePromotion(request, "admin@test.com")).Returns(promotion);

        var result = _controller.CreatePromotion(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        var body = result.Value as PromotionCreateResponse;
        Assert.IsNotNull(body);
        Assert.AreEqual("Promo Verano", body.Name);
        Assert.AreEqual(20, body.DiscountPercentage);
        _mockService.VerifyAll();
    }

    [TestMethod]
    public void UpdatePromotion_ValidRequest_ShouldReturnOk()
    {
        var id = Guid.NewGuid();
        var request = new PromotionCreateRequest
        {
            Name = "Promo Actualizada",
            DiscountPercentage = 25,
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            ProductCodes = []
        };

        var promotion = CreateTestPromotion("Promo Actualizada", 25);
        _mockService.Setup(s => s.UpdatePromotion(id, request, "admin@test.com")).Returns(promotion);

        var result = _controller.UpdatePromotion(id, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var body = result.Value as PromotionCreateResponse;
        Assert.IsNotNull(body);
        Assert.AreEqual("Promo Actualizada", body.Name);
        Assert.AreEqual(25, body.DiscountPercentage);
        _mockService.VerifyAll();
    }

    [TestMethod]
    public void CreatePromotion_WithNoUserClaims_ShouldUseUnknownUser()
    {
        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 20,
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            ProductCodes = []
        };

        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var promotion = CreateTestPromotion("Promo Verano", 20);
        _mockService.Setup(s => s.CreatePromotion(request, "Unknown")).Returns(promotion);

        _controller.CreatePromotion(request);

        _mockService.VerifyAll();
    }

    [TestMethod]
    public void UpdatePromotion_WithNoUserClaims_ShouldUseUnknownUser()
    {
        var id = Guid.NewGuid();
        var request = new PromotionCreateRequest
        {
            Name = "Promo Actualizada",
            DiscountPercentage = 25,
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            ProductCodes = []
        };

        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var promotion = CreateTestPromotion("Promo Actualizada", 25);
        _mockService.Setup(s => s.UpdatePromotion(id, request, "Unknown")).Returns(promotion);

        _controller.UpdatePromotion(id, request);

        _mockService.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_DateWithoutTime_ShouldAdjustToEndOfDay()
    {
        var date = new DateTime(2024, 5, 15);
        var expectedDate = new DateTime(2024, 5, 15, 23, 59, 59);

        _mockService.Setup(s => s.GetPromotions(expectedDate, null, null)).Returns([]);

        var result = _controller.GetPromotions(date, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockService.VerifyAll();
    }
}
