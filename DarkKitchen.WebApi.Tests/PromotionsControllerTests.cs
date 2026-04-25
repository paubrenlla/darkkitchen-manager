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
    private List<PromotionCreateResponse> _testPromotions = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IPromotionService>();

        _testPromotions =
        [
            new PromotionCreateResponse
            {
                Name = "Black Friday",
                DiscountPercentage = 10,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31),
                Products = ["BURG01"]
            },
            new PromotionCreateResponse
            {
                Name = "Semana de Turismo",
                DiscountPercentage = 15,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31),
                Products = ["BURG02"]
            }

        ];

        _controller = new PromotionsController(_mockService.Object);
    }

    [TestMethod]
    public void GetPromotions_NoFilters_ShouldReturnOkWithPromotions()
    {
        _mockService.Setup(s => s.GetPromotions(null, null, null)).Returns(_testPromotions);

        var result = _controller.GetPromotions(null, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var promotions = (result.Value as IEnumerable<PromotionCreateResponse>)?.ToList();
        Assert.IsNotNull(promotions);
        Assert.AreEqual(2, promotions.Count);
    }

    [TestMethod]
    public void GetPromotions_WithFilters_ShouldPassFiltersToService()
    {
        _mockService.Setup(s => s.GetPromotions(null, "Combo burgers", "BURG01")).Returns([_testPromotions[0]]);

        var result = _controller.GetPromotions(null, "Combo burgers", "BURG01") as OkObjectResult;

        Assert.IsNotNull(result);
        _mockService.Verify(s => s.GetPromotions(null, "Combo burgers", "BURG01"), Times.Once);
    }

    [TestMethod]
    public void GetPromotions_NoResults_ShouldReturnOkWithEmptyList()
    {
        _mockService.Setup(s => s.GetPromotions(null, "Desayunos", null)).Returns([]);

        var result = _controller.GetPromotions(null, "Desayunos", null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void CreatePromotion_ValidRequest_ShouldReturn201WithResponse()
    {
        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 20,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = ["BURG01"]
        };
        var expected = new PromotionCreateResponse
        {
            Name = "Promo Verano",
            DiscountPercentage = 20,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            Products = ["BURG01"]
        };
        _mockService.Setup(s => s.CreatePromotion(request)).Returns(expected);

        var result = _controller.CreatePromotion(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        Assert.AreEqual(expected, result.Value);
    }

    [TestMethod]
    public void CreatePromotion_InvalidProductCode_ShouldReturnBadRequest()
    {
        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 20,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31),
            ProductCodes = ["INVALID99"]
        };
        _mockService.Setup(s => s.CreatePromotion(request))
            .Throws(new ArgumentException("Uno o más códigos de producto no son válidos."));

        var result = _controller.CreatePromotion(request) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }
}
