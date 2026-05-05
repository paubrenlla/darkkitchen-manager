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
    private OrdersController _controller = null!;
    private Mock<IOrderService> _mockOrderService = null!;

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
            new(ClaimTypes.NameIdentifier, callerId.ToString()), new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
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
            Total = 100m
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
                Country = "Uruguay"
            },
            Items = [new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 2 }]
        };

        var response = new OrderCreateResponse
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            OrderNumber = 1,
            Subtotal = 100m,
            ShippingCost = 50m,
            Total = 183m
        };

        _mockOrderService.Setup(s => s.CreateOrder(clientId, request)).Returns(response);

        var result = _controller.CreateOrder(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
    }

    [TestMethod]
    public void UpdateStatus_Preparado_AsPreparador_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();
        OrderDetailResponse detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.Prepare(orderId));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result =
            _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Preparado" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.Prepare(orderId), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_Preparado_AsAdministrativo_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Administrativo");
        var orderId = Guid.NewGuid();
        OrderDetailResponse detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.Prepare(orderId));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result =
            _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Preparado" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.Prepare(orderId), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_Preparado_AsCliente_ReturnsForbid()
    {
        SetCallerContext(Guid.NewGuid(), "Cliente");
        var orderId = Guid.NewGuid();

        var result =
            _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Preparado" }) as ForbidResult;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void UpdateStatus_Demorado_AsPreparador_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();
        OrderDetailResponse detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.Delay(orderId));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result =
            _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Demorado" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.Delay(orderId), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_Demorado_AsAdministrativo_ReturnsForbid()
    {
        SetCallerContext(Guid.NewGuid(), "Administrativo");
        var orderId = Guid.NewGuid();

        var result =
            _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Demorado" }) as ForbidResult;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void UpdateStatus_Demorado_AsCliente_ReturnsForbid()
    {
        SetCallerContext(Guid.NewGuid(), "Cliente");
        var orderId = Guid.NewGuid();

        var result =
            _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Demorado" }) as ForbidResult;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void UpdateStatus_Cancelado_AsAdministrativo_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Administrativo");
        var orderId = Guid.NewGuid();
        OrderDetailResponse detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.Cancel(orderId));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result =
            _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Cancelado" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.Cancel(orderId), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_Cancelado_AsPreparador_ReturnsForbid()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();

        var result =
            _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Cancelado" }) as ForbidResult;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void UpdateStatus_EnCamino_AsPreparador_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();
        OrderDetailResponse detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.Ship(orderId));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result =
            _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "EnCamino" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.Ship(orderId), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_EnCamino_AsAdministrativo_ReturnsForbid()
    {
        SetCallerContext(Guid.NewGuid(), "Administrativo");
        var orderId = Guid.NewGuid();

        var result =
            _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "EnCamino" }) as ForbidResult;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void UpdateStatus_Entregado_AsPreparador_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();
        OrderDetailResponse detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.Deliver(orderId));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result =
            _controller.UpdateStatus(orderId, new OrderStatusUpdateRequest { Status = "Entregado" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.Deliver(orderId), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_NoEntregado_AsPreparador_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();
        OrderDetailResponse detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.NotDelivered(orderId));
        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result =
            _controller.UpdateStatus(orderId,
                new OrderStatusUpdateRequest { Status = "NoEntregado" }) as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.NotDelivered(orderId), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_InvalidState_ReturnsBadRequest()
    {
        SetCallerContext(Guid.NewGuid(), "Administrativo");

        var result =
            _controller.UpdateStatus(Guid.NewGuid(), new OrderStatusUpdateRequest { Status = "EstadoInvalido" }) as
                BadRequestObjectResult;

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
            new()
            {
                Id = Guid.NewGuid(),
                OrderNumber = 1,
                ClientId = clientId,
                CreatedAt = DateTime.Now,
                Status = "Pending",
                Total = 100m,
                ProductCount = 2
            }
        };

        _mockOrderService.Setup(s => s.GetOrdersByClient(clientId, null, null, null)).Returns(orders);

        var result = _controller.GetOrders(null, null, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockOrderService.Verify(s => s.GetOrdersByClient(clientId, null, null, null), Times.Once);
    }

    [TestMethod]
    public void GetOrders_AsCliente_ReturnsNoContentWhenEmpty()
    {
        var clientId = Guid.NewGuid();
        SetCallerContext(clientId, "Cliente");

        _mockOrderService.Setup(s => s.GetOrdersByClient(clientId, null, null, null)).Returns([]);

        var result = _controller.GetOrders(null, null, null, null) as NoContentResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(204, result.StatusCode);
    }

    [TestMethod]
    public void GetOrders_AsPreparador_WithDates_ReturnsAllOrders()
    {
        var preparadorId = Guid.NewGuid();
        SetCallerContext(preparadorId, "Preparador");

        DateTime from = DateTime.Now.AddDays(-7);
        DateTime to = DateTime.Now;

        var orders = new List<OrderListResponse>
        {
            new()
            {
                Id = Guid.NewGuid(),
                OrderNumber = 1,
                ClientId = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Status = "Pending",
                Total = 100m,
                ProductCount = 2
            }
        };

        _mockOrderService.Setup(s => s.GetOrdersByStatus(from, to, null, null)).Returns(orders);

        var result = _controller.GetOrders(from, to, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockOrderService.Verify(s => s.GetOrdersByStatus(from, to, null, null), Times.Once);
    }

    [TestMethod]
    public void GetOrders_AsPreparador_ReturnsNoContentWhenEmpty()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var from = DateTime.Now.AddDays(-7);
        var to = DateTime.Now;

        _mockOrderService.Setup(s => s.GetOrdersByStatus(from, to, null, null)).Returns([]);

        var result = _controller.GetOrders(from, to, null, null) as NoContentResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(204, result.StatusCode);
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

        var orders = new List<OrderListResponse>
        {
            new()
            {
                Id = Guid.NewGuid(),
                OrderNumber = 1,
                ClientId = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Status = "Pending",
                Total = 100m,
                ProductCount = 2
            }
        };
        _mockOrderService.Setup(s => s.GetOrdersByStatus(from, to, "Pending", "Rivera"))
            .Returns(orders);

        var result = _controller.GetOrders(from, to, "Pending", "Rivera") as OkObjectResult;

        Assert.IsNotNull(result);
        _mockOrderService.Verify(s => s.GetOrdersByStatus(from, to, "Pending", "Rivera"), Times.Once);
    }

    [TestMethod]
    public void GetOrderDetail_AsPreparador_Exists_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Preparador");
        var orderId = Guid.NewGuid();
        OrderDetailResponse detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result = _controller.GetOrderDetail(orderId) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void GetOrderDetail_AsAdministrativo_Exists_ReturnsOk()
    {
        SetCallerContext(Guid.NewGuid(), "Administrativo");
        var orderId = Guid.NewGuid();
        OrderDetailResponse detailResponse = BuildDetailResponse();

        _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(detailResponse);

        var result = _controller.GetOrderDetail(orderId) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }
}
