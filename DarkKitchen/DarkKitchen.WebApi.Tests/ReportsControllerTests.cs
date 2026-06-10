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
        _mockReportService = new Mock<IReportService>(MockBehavior.Strict);
        _controller = new ReportsController(_mockReportService.Object);

        List<Claim> claims = [new Claim(ClaimTypes.Role, "Administrativo")];
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

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

        var result = _controller.GetTopProducts(from, to) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockReportService.VerifyAll();
    }

    [TestMethod]
    public void GetTopProducts_NoResults_ReturnsOkWithEmptyList()
    {
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        _mockReportService.Setup(s => s.GetTopProducts(from, to)).Returns([]);

        var result = _controller.GetTopProducts(from, to) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockReportService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetTopProducts_FromAfterTo_ShouldPropagateException()
    {
        DateTime from = DateTime.Now;
        DateTime to = DateTime.Now.AddDays(-30);

        _mockReportService.Setup(s => s.GetTopProducts(from, to))
            .Throws(new ArgumentException("La fecha de inicio no puede ser posterior a la fecha de fin."));

        _controller.GetTopProducts(from, to);

        _mockReportService.VerifyAll();
    }

    [TestMethod]
    public void GetSalesReport_ReturnsOk()
    {
        var report = new SalesReportResponse
        {
            Periods =
            [
                new SalesPeriodResponse
                {
                    Year = 2026,
                    Month = 1,
                    Clients = [new SalesClientResponse { ClientName = "Juan Perez", Total = 5000m }],
                    PeriodTotal = 5000m,
                },
            ],
            GrandTotal = 5000m,
        };

        _mockReportService.Setup(s => s.GetSalesReport()).Returns(report);

        var result = _controller.GetSalesReport() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockReportService.VerifyAll();
    }

    [TestMethod]
    public void GetSalesReport_EmptyReport_ReturnsOk()
    {
        var emptyReport = new SalesReportResponse { Periods = [], GrandTotal = 0 };

        _mockReportService.Setup(s => s.GetSalesReport()).Returns(emptyReport);

        var result = _controller.GetSalesReport() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockReportService.VerifyAll();
    }

    [TestMethod]
    public void GetTopProducts_ToDateWithoutTime_ShouldAdjustToEndOfDay()
    {
        var from = DateTime.Now.AddDays(-30);
        var to = new DateTime(2024, 5, 15);
        var expectedTo = new DateTime(2024, 5, 15, 23, 59, 59);

        _mockReportService.Setup(s => s.GetTopProducts(from, expectedTo)).Returns([]);

        var result = _controller.GetTopProducts(from, to) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockReportService.VerifyAll();
    }
}
