using DarkKitchen.Domain.Audit;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class AuditsControllerTests
{
    private AuditsController _controller = null!;
    private Mock<IAuditService> _mockService = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IAuditService>(MockBehavior.Strict);
        _controller = new AuditsController(_mockService.Object);
    }

    [TestMethod]
    public void GetAudits_ValidFilters_ShouldReturnOk()
    {
        var from = DateTime.UtcNow.AddDays(-1);
        var to = DateTime.UtcNow;
        var auditLogs = new List<AuditLog>
        {
            new AuditLog { EntityName = "Product" }
        };

        _mockService.Setup(s => s.GetAudits(from, to, null, null)).Returns(auditLogs);

        var result = _controller.GetAudits(from, to, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        var body = result.Value as List<AuditLogResponse>;
        Assert.IsNotNull(body);
        Assert.AreEqual("Product", body[0].EntityName);
        _mockService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetAudits_ServiceThrowsArgumentException_ShouldPropagate()
    {
        var from = DateTime.UtcNow;
        var to = DateTime.UtcNow.AddDays(-1);

        _mockService.Setup(s => s.GetAudits(from, to, null, null))
            .Throws(new ArgumentException("La fecha 'desde' no puede ser mayor que la fecha 'hasta'."));

        _controller.GetAudits(from, to, null, null);

        _mockService.VerifyAll();
    }

    [TestMethod]
    public void GetAudits_ToDateWithoutTime_ShouldAdjustToEndOfDay()
    {
        var from = DateTime.UtcNow.AddDays(-1);
        var to = new DateTime(2024, 5, 15);
        var expectedTo = new DateTime(2024, 5, 15, 23, 59, 59);

        _mockService.Setup(s => s.GetAudits(from, expectedTo, null, null)).Returns([]);

        var result = _controller.GetAudits(from, to, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockService.VerifyAll();
    }
}
