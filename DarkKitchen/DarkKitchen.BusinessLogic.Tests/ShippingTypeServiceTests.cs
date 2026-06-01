using DarkKitchen.Domain.Orders.Delivery;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class ShippingTypeServiceTests
{
    private Mock<IShippingTypeRepository> _repositoryMock = null!;
    private ShippingTypeService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<IShippingTypeRepository>(MockBehavior.Strict);
        _service = new ShippingTypeService(_repositoryMock.Object);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllShippingTypes()
    {
        var types = new List<ShippingType>
        {
            new ShippingType("Express", 150m),
            new ShippingType("Dia siguiente", 80m),
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(types);

        var result = _service.GetAll().ToList();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Express", result[0].Name);
        Assert.AreEqual(150m, result[0].Cost);
        _repositoryMock.VerifyAll();
    }

    [TestMethod]
    public void Create_ValidRequest_ShouldAddAndReturn()
    {
        _repositoryMock.Setup(r => r.Add(It.IsAny<ShippingType>()));

        var request = new ShippingTypeRequest { Name = "Express", Cost = 150m };

        var result = _service.Create(request);

        Assert.AreEqual("Express", result.Name);
        Assert.AreEqual(150m, result.Cost);
        _repositoryMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Create_WithEmptyName_ShouldThrow()
    {
        var request = new ShippingTypeRequest { Name = string.Empty, Cost = 150m };

        _service.Create(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Create_WithNegativeCost_ShouldThrow()
    {
        var request = new ShippingTypeRequest { Name = "Express", Cost = -1m };

        _service.Create(request);
    }

    [TestMethod]
    public void Update_ExistingId_ShouldUpdateAndReturn()
    {
        var existing = new ShippingType("Express", 150m);
        _repositoryMock.Setup(r => r.GetById(existing.Id)).Returns(existing);
        _repositoryMock.Setup(r => r.Update(It.IsAny<ShippingType>()));

        var request = new ShippingTypeRequest { Name = "Express Premium", Cost = 250m };

        var result = _service.Update(existing.Id, request);

        Assert.AreEqual("Express Premium", result.Name);
        Assert.AreEqual(250m, result.Cost);
        _repositoryMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Update_NonExistingId_ShouldThrow()
    {
        var unknownId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(unknownId)).Returns((ShippingType?)null);

        _service.Update(unknownId, new ShippingTypeRequest { Name = "Express", Cost = 100m });

        _repositoryMock.VerifyAll();
    }

    [TestMethod]
    public void Delete_ExistingId_ShouldCallRepository()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.Delete(id));

        _service.Delete(id);

        _repositoryMock.VerifyAll();
    }
}
