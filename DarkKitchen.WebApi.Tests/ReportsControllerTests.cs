using System.Security.Claims;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class ReportsControllerTests
{
    private Mock<IReportService> _mockReportService = null!;
    private ReportsController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockReportService = new Mock<IReportService>();
        _controller = new ReportsController(_mockReportService.Object);

        List<Claim> claims = [new Claim(ClaimTypes.Role, "Administrativo")];
        ClaimsIdentity identity = new ClaimsIdentity(claims, "Test");
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal },
        };
    }

    [TestMethod]
    public void GetTopProducts_WithValidDates_ReturnsOk()
    {
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        List<TopProductResponse> topProducts =
        [
            new TopProductResponse
            {
                Code = "BURG01",
                Name = "Hamburguesa Clasica",
                QuantitySold = 50,
                Images = ["https://example.com/photo.jpg"],
            },
        ];

        _mockReportService.Setup(s => s.GetTopProducts(from, to)).Returns(topProducts);

        OkObjectResult? result = _controller.GetTopProducts(from, to) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void GetTopProducts_NoResults_ReturnsOkWithEmptyList()
    {
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        _mockReportService.Setup(s => s.GetTopProducts(from, to)).Returns([]);

        OkObjectResult? result = _controller.GetTopProducts(from, to) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void GetTopProducts_FromAfterTo_ReturnsBadRequest()
    {
        DateTime from = DateTime.Now;
        DateTime to = DateTime.Now.AddDays(-30);

        BadRequestObjectResult? result = _controller.GetTopProducts(from, to) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }
}
