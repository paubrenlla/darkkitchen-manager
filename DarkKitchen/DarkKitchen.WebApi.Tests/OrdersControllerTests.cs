using System.Security.Claims;
using DarkKitchen.Domain.Orders;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class OrdersControllerTests
{
    private OrdersController _controller = null!;
    private Mock<IOrderService> _mockOrderService = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockOrderService = new Mock<IOrderService>(MockBehavior.Strict);
        _controller = new OrdersController(_mockOrderService.Object);
        SetCallerContext(Guid.NewGuid(), "Administrativo");
    }

    private void SetCallerContext(Guid callerId, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, callerId.ToString()),
            new(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "Test");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    private static Order CreateTestOrder(Guid? clientId = null)
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
        return new Order(clientId ?? Guid.NewGuid(), address, "Express", items, 0m);
    }

    [TestMethod]
    public void CreateOrder_ValidRequest_Returns201()
    {
        var clientId = Guid.NewGuid();
        SetCallerContext(clientId, "Cliente");

        var request = new OrderCreateRequest
        {
            DeliveryType = "Express",
            Address = new OrderAddressDto { Street = "Rivera", Number = "1234", City = "Montevideo", Country = "Uruguay" },
            Items = [new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 2 }]
        };

        var order = CreateTestOrder(clientId);
        _mockOrderService.Setup(s => s.CreateOrder(clientId, request)).Returns(order);

        var result = _controller.CreateOrder(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        _mockOrderService.VerifyAll();
    }

    [TestMethod]
    public void UpdateStatus_ValidTransition_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder();

        _mockOrderService.Setup(s => s.UpdateOrderStatus(orderId, "Preparado"));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(order);

        var result = _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Preparado" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateStatus_InvalidStatus_PropagatesException()
    {
        SetCallerContext(Guid.NewGuid(), "Administrativo");
        var orderId = Guid.NewGuid();

        _mockOrderService.Setup(s => s.UpdateOrderStatus(orderId, "EstadoInvalido"))
            .Throws(new ArgumentException("Estado 'EstadoInvalido' no válido."));

        _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "EstadoInvalido" });

        _mockOrderService.VerifyAll();
    }

    [TestMethod]
    public void GetOrders_AsCliente_ReturnsOk()
    {
        var clientId = Guid.NewGuid();
        SetCallerContext(clientId, "Cliente");

        var orders = new List<OrderListResponse>
        {
            new() { Id = Guid.NewGuid(), Status = "Pending", Total = 100m }
        };

        _mockOrderService.Setup(s => s.GetOrders(clientId, "Cliente", It.IsAny<OrderFilter>()))
            .Returns(orders);

        var result = _controller.GetOrders(null, null, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockOrderService.VerifyAll();
    }

    [TestMethod]
    public void GetOrders_AsPreparador_WithDates_ReturnsOk()
    {
        var preparadorId = Guid.NewGuid();
        SetCallerContext(preparadorId, "Preparador");

        var from = DateTime.Now.AddDays(-7);
        var to = DateTime.Now;
        var orders = new List<OrderListResponse>
        {
            new() { Id = Guid.NewGuid(), Status = "Pending", Total = 100m }
        };

        _mockOrderService.Setup(s => s.GetOrders(preparadorId, "Preparador", It.IsAny<OrderFilter>()))
            .Returns(orders);

        var result = _controller.GetOrders(from, to, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockOrderService.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetOrders_AsPreparador_WithoutDates_PropagatesException()
    {
        var preparadorId = Guid.NewGuid();
        SetCallerContext(preparadorId, "Preparador");

        _mockOrderService.Setup(s => s.GetOrders(preparadorId, "Preparador", It.IsAny<OrderFilter>()))
            .Throws(new ArgumentException("El rango de fechas es obligatorio para el preparador."));

        _controller.GetOrders(null, null, null, null);

        _mockOrderService.VerifyAll();
    }

    [TestMethod]
    public void GetOrderDetail_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();
        var order = CreateTestOrder();

        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(order);

        var result = _controller.GetOrderDetail(orderId) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockOrderService.VerifyAll();
    }

    [TestMethod]
    public void GetOrders_WithNoRoleClaim_ShouldCallGetOrdersAsClient()
    {
        var callerId = Guid.NewGuid();
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, callerId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };

        var orders = new List<OrderListResponse>
        {
            new() { Id = Guid.NewGuid(), Status = "Pending", Total = 100m }
        };

        _mockOrderService.Setup(s => s.GetOrders(callerId, null, It.IsAny<OrderFilter>())).Returns(orders);

        var result = _controller.GetOrders(null, null, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.VerifyAll();
    }

    [TestMethod]
    public void GetOrders_ToDateWithoutTime_ShouldAdjustToEndOfDay()
    {
        var callerId = Guid.NewGuid();
        SetCallerContext(callerId, "Cliente");

        var fromDate = new DateTime(2024, 5, 10);
        var toDate = new DateTime(2024, 5, 15);
        var expectedToDate = new DateTime(2024, 5, 15, 23, 59, 59);

        var orders = new List<OrderListResponse>();

        _mockOrderService.Setup(s => s.GetOrders(callerId, "Cliente", It.Is<OrderFilter>(f => f.To == expectedToDate)))
            .Returns(orders);

        var result = _controller.GetOrders(fromDate, toDate, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.VerifyAll();
    }
}
