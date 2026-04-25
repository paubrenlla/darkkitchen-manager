using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
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
}
