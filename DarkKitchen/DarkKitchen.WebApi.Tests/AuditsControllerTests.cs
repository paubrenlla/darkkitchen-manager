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
        _mockService = new Mock<IAuditService>();
        _controller = new AuditsController(_mockService.Object);
    }

    [TestMethod]
    public void GetAudits_ValidFilters_ShouldReturnOk()
    {
        var from = DateTime.UtcNow.AddDays(-1);
        var to = DateTime.UtcNow;
        var expectedAudits = new List<AuditLogResponse> { new AuditLogResponse { EntityName = "Product" } };

        _mockService.Setup(s => s.GetAudits(from, to, null, null)).Returns(expectedAudits);

        var result = _controller.GetAudits(from, to, null, null);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(expectedAudits, okResult.Value);
    }

    [TestMethod]
    public void GetAudits_MissingFilters_ShouldReturnBadRequest()
    {
        var result = _controller.GetAudits(null, null, null, null);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual("Los filtros 'from' y 'to' son obligatorios.", badRequestResult.Value);
    }

    [TestMethod]
    public void GetAudits_ServiceThrowsArgumentException_ShouldReturnBadRequest()
    {
        var from = DateTime.UtcNow;
        var to = DateTime.UtcNow.AddDays(-1);
        var errorMessage = "La fecha 'desde' no puede ser mayor que la fecha 'hasta'.";

        _mockService.Setup(s => s.GetAudits(from, to, null, null)).Throws(new ArgumentException(errorMessage));

        var result = _controller.GetAudits(from, to, null, null);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(errorMessage, badRequestResult.Value);
    }
}
