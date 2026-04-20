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
}
