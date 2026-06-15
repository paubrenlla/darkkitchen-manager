using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class PluginsControllerTests
{
    private Mock<IPluginService> _mockService = null!;
    private PluginsController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IPluginService>(MockBehavior.Strict);
        _controller = new PluginsController(_mockService.Object);
    }

    [TestMethod]
    public void GetImporters_ShouldReturnList()
    {
        _mockService.Setup(s => s.GetAvailableImporters())
            .Returns(["CSV Importer", "JSON Importer", "XML Importer"]);

        var result = _controller.GetImporters() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var list = result.Value as List<string>;
        Assert.IsNotNull(list);
        Assert.AreEqual(3, list.Count);
        _mockService.VerifyAll();
    }

    [TestMethod]
    public void GetImporters_NoPlugins_ShouldReturnEmptyList()
    {
        _mockService.Setup(s => s.GetAvailableImporters()).Returns([]);

        var result = _controller.GetImporters() as OkObjectResult;

        Assert.IsNotNull(result);
        var list = result.Value as List<string>;
        Assert.IsNotNull(list);
        Assert.AreEqual(0, list.Count);
        _mockService.VerifyAll();
    }
}
