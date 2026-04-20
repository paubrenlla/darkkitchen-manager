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
    private Mock<IOrderService> _mockOrderService = null!;
    private OrdersController _controller = null!;
    private Guid _clientId;

    [TestInitialize]
    public void Setup()
    {
        _mockOrderService = new Mock<IOrderService>();
        _controller = new OrdersController(_mockOrderService.Object);
        _clientId = Guid.NewGuid();
    }

   [TestMethod]
   public void CreateOrder_ValidRequest_Returns201()
   {
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

       var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
       var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
       var order = new Order(_clientId, address, DeliveryType.Express, items);
       order.AssignOrderNumber(1);

       _mockOrderService.Setup(s => s.CreateOrder(
           _clientId,
           It.IsAny<Address>(),
           DeliveryType.Express,
           It.IsAny<List<OrderItem>>())).Returns(order);

       var result = _controller.CreateOrder(request, _clientId) as ObjectResult;

       Assert.IsNotNull(result);
       Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
   }

   [TestMethod]
   public void CreateOrder_InvalidDeliveryType_ReturnsBadRequest()
   {
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

       var result = _controller.CreateOrder(request, _clientId) as BadRequestObjectResult;

       Assert.IsNotNull(result);
       Assert.AreEqual(400, result.StatusCode);
   }

   [TestMethod]
   public void CreateOrder_ServiceThrowsArgumentException_ReturnsBadRequest()
   {
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
           Items = [new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 1 }],
       };

       _mockOrderService.Setup(s => s.CreateOrder(
               It.IsAny<Guid>(),
               It.IsAny<Address>(),
               It.IsAny<DeliveryType>(),
               It.IsAny<List<OrderItem>>()))
           .Throws(new ArgumentException("El pedido debe tener al menos un producto."));

       var result = _controller.CreateOrder(request, _clientId) as BadRequestObjectResult;

       Assert.IsNotNull(result);
       Assert.AreEqual(400, result.StatusCode);
   }

   [TestMethod]
   public void UpdateStatus_Cancelado_ReturnsOk()
   {
       var orderId = Guid.NewGuid();
       var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
       var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
       var order = new Order(_clientId, address, DeliveryType.Express, items);

       _mockOrderService.Setup(s => s.Cancel(orderId));
       _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(order);

       var request = new OrderStatusUpdateRequest { Status = "Cancelado" };
       var result = _controller.UpdateStatus(orderId, request) as OkObjectResult;

       Assert.IsNotNull(result);
       _mockOrderService.Verify(s => s.Cancel(orderId), Times.Once);
   }

   [TestMethod]
   public void UpdateStatus_Entregado_ReturnsOk()
   {
       var orderId = Guid.NewGuid();
       var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
       var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
       var order = new Order(_clientId, address, DeliveryType.Express, items);

       _mockOrderService.Setup(s => s.Deliver(orderId));
       _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(order);

       var request = new OrderStatusUpdateRequest { Status = "Entregado" };
       var result = _controller.UpdateStatus(orderId, request) as OkObjectResult;

       Assert.IsNotNull(result);
       _mockOrderService.Verify(s => s.Deliver(orderId), Times.Once);
   }

   [TestMethod]
   public void UpdateStatus_Preparado_ReturnsOk()
   {
       var orderId = Guid.NewGuid();
       var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
       var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
       var order = new Order(_clientId, address, DeliveryType.Express, items);

       _mockOrderService.Setup(s => s.Prepare(orderId));
       _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(order);

       var request = new OrderStatusUpdateRequest { Status = "Preparado" };
       var result = _controller.UpdateStatus(orderId, request) as OkObjectResult;

       Assert.IsNotNull(result);
       _mockOrderService.Verify(s => s.Prepare(orderId), Times.Once);
   }

   [TestMethod]
   public void UpdateStatus_EnCamino_ReturnsOk()
   {
       var orderId = Guid.NewGuid();
       var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
       var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
       var order = new Order(_clientId, address, DeliveryType.Express, items);

       _mockOrderService.Setup(s => s.Ship(orderId));
       _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(order);

       var request = new OrderStatusUpdateRequest { Status = "EnCamino" };
       var result = _controller.UpdateStatus(orderId, request) as OkObjectResult;

       Assert.IsNotNull(result);
       _mockOrderService.Verify(s => s.Ship(orderId), Times.Once);
   }

   [TestMethod]
   public void UpdateStatus_NoEntregado_ReturnsOk()
   {
       var orderId = Guid.NewGuid();
       var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
       var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
       var order = new Order(_clientId, address, DeliveryType.Express, items);

       _mockOrderService.Setup(s => s.NotDelivered(orderId));
       _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(order);

       var request = new OrderStatusUpdateRequest { Status = "NoEntregado" };
       var result = _controller.UpdateStatus(orderId, request) as OkObjectResult;

       Assert.IsNotNull(result);
       _mockOrderService.Verify(s => s.NotDelivered(orderId), Times.Once);
   }

   [TestMethod]
   public void UpdateStatus_InvalidState_ReturnsBadRequest()
   {
       var request = new OrderStatusUpdateRequest { Status = "EstadoInvalido" };

       var result = _controller.UpdateStatus(Guid.NewGuid(), request) as BadRequestObjectResult;

       Assert.IsNotNull(result);
       Assert.AreEqual(400, result.StatusCode);
   }

   [TestMethod]
   public void UpdateStatus_OrderNotFound_ReturnsNotFound()
   {
       var orderId = Guid.NewGuid();

       _mockOrderService.Setup(s => s.Prepare(orderId))
           .Throws(new KeyNotFoundException("Pedido no encontrado."));

       var request = new OrderStatusUpdateRequest { Status = "Preparado" };
       var result = _controller.UpdateStatus(orderId, request) as NotFoundObjectResult;

       Assert.IsNotNull(result);
       Assert.AreEqual(404, result.StatusCode);
   }

   [TestMethod]
   public void UpdateStatus_InvalidTransition_ReturnsBadRequest()
   {
       var orderId = Guid.NewGuid();

       _mockOrderService.Setup(s => s.Ship(orderId))
           .Throws(new InvalidOperationException("No se puede enviar en estado Pending"));

       var request = new OrderStatusUpdateRequest { Status = "EnCamino" };
       var result = _controller.UpdateStatus(orderId, request) as BadRequestObjectResult;

       Assert.IsNotNull(result);
       Assert.AreEqual(400, result.StatusCode);
   }

   [TestMethod]
   public void GetOrderDetail_Exists_ReturnsOk()
   {
       var orderId = Guid.NewGuid();
       var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
       var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
       var order = new Order(_clientId, address, DeliveryType.Express, items);

       _mockOrderService.Setup(s => s.GetOrderById(orderId)).Returns(order);

       var result = _controller.GetOrderDetail(orderId) as OkObjectResult;

       Assert.IsNotNull(result);
       Assert.AreEqual(200, result.StatusCode);
   }

   [TestMethod]
   public void GetOrderDetail_NotFound_ReturnsNotFound()
   {
       _mockOrderService.Setup(s => s.GetOrderById(It.IsAny<Guid>()))
           .Throws(new KeyNotFoundException("Pedido no encontrado."));

       var result = _controller.GetOrderDetail(Guid.NewGuid()) as NotFoundObjectResult;

       Assert.IsNotNull(result);
       Assert.AreEqual(404, result.StatusCode);
   }

   [TestMethod]
   public void GetOrders_ReturnsOkWithList()
   {
       var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
       var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
       var orders = new List<Order> { new(_clientId, address, DeliveryType.Express, items) };

       _mockOrderService.Setup(s => s.GetOrdersByClient(_clientId, null, null, null)).Returns(orders);

       var result = _controller.GetOrders(_clientId, null, null, null) as OkObjectResult;

       Assert.IsNotNull(result);
       Assert.AreEqual(200, result.StatusCode);
   }
}
