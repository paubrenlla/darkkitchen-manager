using System.Security.Claims;
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
    private Mock<IOrderService> _mockOrderService = null!;
    private OrdersController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockOrderService = new Mock<IOrderService>();
        _controller = new OrdersController(_mockOrderService.Object);
        SetCallerContext(Guid.NewGuid(), "Administrativo");
    }

    private void SetCallerContext(Guid callerId, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, callerId.ToString()),
            new Claim(ClaimTypes.Role, role),
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal },
        };
    }

    private static OrderDetailResponse BuildDetailResponse()
    {
        return new OrderDetailResponse
        {
            Id = Guid.NewGuid(),
            OrderNumber = 1,
            ClientId = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Status = "Pending",
            Items = [],
            Total = 100m,
        };
    }

    [TestMethod]
    public void CreateOrder_ValidRequest_Returns201()
    {
        var clientId = Guid.NewGuid();
        SetCallerContext(clientId, "Cliente");

        var request = new OrderCreateRequest
        {
            DeliveryType = "Express",
            Address = new OrderAddressDto
            {
                Street = "Rivera",
                Number = "1234",
                City = "Montevideo",
                Country = "Uruguay",
            },
            Items = [new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 2 }],
        };

        var response = new OrderCreateResponse
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            OrderNumber = 1,
            Subtotal = 100m,
            ShippingCost = 50m,
            Total = 183m,
        };

        _mockOrderService.Setup(s => s.CreateOrder(clientId, request)).Returns(response);

        var result = _controller.CreateOrder(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
    }

    [TestMethod]
    public void CreateOrder_ServiceThrowsArgumentException_ReturnsBadRequest()
    {
        var clientId = Guid.NewGuid();
        SetCallerContext(clientId, "Cliente");

        var request = new OrderCreateRequest
        {
            DeliveryType = "InvalidType",
            Address = new OrderAddressDto
            {
                Street = "Rivera",
                Number = "1234",
                City = "Montevideo",
                Country = "Uruguay",
            },
            Items = [new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 1 }],
        };

        _mockOrderService.Setup(s => s.CreateOrder(clientId, request))
            .Throws(new ArgumentException("Tipo de entrega inválido."));

        var result = _controller.CreateOrder(request) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void UpdateStatus_Preparado_AsPreparador_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();
        var detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.Prepare(orderId));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result = _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Preparado" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.Prepare(orderId), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_Preparado_AsAdministrativo_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Administrativo");
        var orderId = Guid.NewGuid();
        var detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.Prepare(orderId));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result = _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Preparado" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.Prepare(orderId), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_Preparado_AsCliente_ReturnsForbid()
    {
        SetCallerContext(Guid.NewGuid(), "Cliente");
        var orderId = Guid.NewGuid();

        var result = _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Preparado" }) as ForbidResult;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void UpdateStatus_Cancelado_AsAdministrativo_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Administrativo");
        var orderId = Guid.NewGuid();
        var detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.Cancel(orderId));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result = _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Cancelado" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.Cancel(orderId), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_Cancelado_AsPreparador_ReturnsForbid()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();

        var result = _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Cancelado" }) as ForbidResult;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void UpdateStatus_EnCamino_AsPreparador_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();
        var detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.Ship(orderId));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result = _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "EnCamino" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.Ship(orderId), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_EnCamino_AsAdministrativo_ReturnsForbid()
    {
        SetCallerContext(Guid.NewGuid(), "Administrativo");
        var orderId = Guid.NewGuid();

        var result = _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "EnCamino" }) as ForbidResult;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void UpdateStatus_Entregado_AsPreparador_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();
        var detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.Deliver(orderId));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result = _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Entregado" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.Deliver(orderId), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_NoEntregado_AsPreparador_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();
        var detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.NotDelivered(orderId));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result = _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "NoEntregado" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.NotDelivered(orderId), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_InvalidState_ReturnsBadRequest()
    {
        SetCallerContext(Guid.NewGuid(), "Administrativo");

        var result = _controller.UpdateStatus(Guid.NewGuid(), new OrderStatusUpdateRequest { Status = "EstadoInvalido" }) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void UpdateStatus_OrderNotFound_ReturnsNotFound()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();

        _mockOrderService.Setup(s => s.Prepare(orderId))
            .Throws(new KeyNotFoundException("Pedido no encontrado."));

        var result = _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Preparado" }) as NotFoundObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public void UpdateStatus_InvalidTransition_ReturnsBadRequest()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();

        _mockOrderService.Setup(s => s.Ship(orderId))
            .Throws(new InvalidOperationException("No se puede enviar en estado Pending"));

        var result = _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "EnCamino" }) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void GetOrders_AsCliente_ReturnsClientOrders()
    {
        var clientId = Guid.NewGuid();
        SetCallerContext(clientId, "Cliente");

        var orders = new List<OrderListResponse>
    {
        new OrderListResponse
        {
            Id = Guid.NewGuid(),
            OrderNumber = 1,
            ClientId = clientId,
            CreatedAt = DateTime.Now,
            Status = "Pending",
            Total = 100m,
            ProductCount = 2,
        },
    };

        _mockOrderService.Setup(s => s.GetOrdersByClient(clientId, null, null, null)).Returns(orders);

        var result = _controller.GetOrders(null, null, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockOrderService.Verify(s => s.GetOrdersByClient(clientId, null, null, null), Times.Once);
    }

    [TestMethod]
    public void GetOrders_AsPreparador_WithDates_ReturnsAllOrders()
    {
        var preparadorId = Guid.NewGuid();
        SetCallerContext(preparadorId, "Preparador");

        var from = DateTime.Now.AddDays(-7);
        var to = DateTime.Now;

        var orders = new List<OrderListResponse>
    {
        new OrderListResponse
        {
            Id = Guid.NewGuid(),
            OrderNumber = 1,
            ClientId = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Status = "Pending",
            Total = 100m,
            ProductCount = 2,
        },
    };

        _mockOrderService.Setup(s => s.GetOrdersByStatus(from, to, null, null)).Returns(orders);

        var result = _controller.GetOrders(from, to, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockOrderService.Verify(s => s.GetOrdersByStatus(from, to, null, null), Times.Once);
    }

    [TestMethod]
    public void GetOrders_AsPreparador_WithoutDates_ReturnsBadRequest()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");

        var result = _controller.GetOrders(null, null, null, null) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void GetOrders_AsPreparador_WithFilters_PassesFiltersToService()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");

        var from = DateTime.Now.AddDays(-7);
        var to = DateTime.Now;

        _mockOrderService.Setup(s => s.GetOrdersByStatus(from, to, "Pending", "Montevideo"))
            .Returns([]);

        var result = _controller.GetOrders(from, to, "Pending", "Montevideo") as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.GetOrdersByStatus(from, to, "Pending", "Montevideo"), Times.Once);
    }
}
