using System.Security.Claims;
using DarkKitchen.Domain.Orders.Delivery;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class ShippingTypesControllerTests
{
    private Mock<IShippingTypeService> _serviceMock = null!;
    private ShippingTypesController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _serviceMock = new Mock<IShippingTypeService>(MockBehavior.Strict);
        _controller = new ShippingTypesController(_serviceMock.Object);
        SetCallerContext(Guid.NewGuid(), "Administrativo");
    }

    private void SetCallerContext(Guid callerId, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, callerId.ToString()),
            new(ClaimTypes.Role, role),
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal },
        };
    }

    [TestMethod]
    public void GetAll_ShouldReturnOkWithList()
    {
        var types = new List<ShippingType>
        {
            new ShippingType("Express", 150m),
            new ShippingType("Dia siguiente", 80m),
        };
        _serviceMock.Setup(s => s.GetAll()).Returns(types);

        var result = _controller.GetAll() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void GetAll_EmptyList_ShouldReturnNoContent()
    {
        _serviceMock.Setup(s => s.GetAll()).Returns([]);

        var result = _controller.GetAll();

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void Create_ValidRequest_ShouldReturn201()
    {
        var request = new ShippingTypeRequest { Name = "Express", Cost = 150m };
        var shippingType = new ShippingType("Express", 150m);
        _serviceMock.Setup(s => s.Create(request)).Returns(shippingType);

        var result = _controller.Create(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void Create_InvalidRequest_ShouldReturnBadRequest()
    {
        var request = new ShippingTypeRequest { Name = string.Empty, Cost = 150m };
        _serviceMock.Setup(s => s.Create(request))
            .Throws(new ArgumentException("El nombre es obligatorio."));

        var result = _controller.Create(request) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void Update_ValidRequest_ShouldReturnOk()
    {
        var id = Guid.NewGuid();
        var request = new ShippingTypeRequest { Name = "Express Premium", Cost = 250m };
        var shippingType = new ShippingType("Express Premium", 250m);
        _serviceMock.Setup(s => s.Update(id, request)).Returns(shippingType);

        var result = _controller.Update(id, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void Update_NotFound_ShouldReturnNotFound()
    {
        var id = Guid.NewGuid();
        var request = new ShippingTypeRequest { Name = "Express", Cost = 150m };
        _serviceMock.Setup(s => s.Update(id, request))
            .Throws(new KeyNotFoundException("Tipo de envío no encontrado."));

        var result = _controller.Update(id, request) as NotFoundObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void Delete_ExistingId_ShouldReturnNoContent()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.Delete(id));

        var result = _controller.Delete(id) as NoContentResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(204, result.StatusCode);
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void Delete_NotFound_ShouldReturnNotFound()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.Delete(id))
            .Throws(new KeyNotFoundException("Tipo de envío no encontrado."));

        var result = _controller.Delete(id) as NotFoundObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
        _serviceMock.VerifyAll();
    }
}
