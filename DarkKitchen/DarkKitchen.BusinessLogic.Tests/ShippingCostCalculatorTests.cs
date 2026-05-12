using DarkKitchen.Domain.Orders.Delivery;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class ShippingCostCalculatorTests
{
    private Mock<IShippingTypeRepository> _repositoryMock = null!;
    private ShippingCostCalculator _calculator = null!;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<IShippingTypeRepository>();
        _calculator = new ShippingCostCalculator(_repositoryMock.Object);
    }

    [TestMethod]
    public void CalculateShippingCost_ExistingType_ShouldReturnCost()
    {
        var shippingType = new ShippingType("Express", 150m);
        _repositoryMock.Setup(r => r.GetByName("Express")).Returns(shippingType);

        var result = _calculator.CalculateShippingCost("Express");

        Assert.AreEqual(150m, result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CalculateShippingCost_NonExistingType_ShouldThrow()
    {
        _repositoryMock.Setup(r => r.GetByName("NoExiste")).Returns((ShippingType?)null);

        _calculator.CalculateShippingCost("NoExiste");
    }

    [TestMethod]
    public void CalculateShippingCost_DifferentTypes_ShouldReturnCorrectCost()
    {
        _repositoryMock.Setup(r => r.GetByName("Express")).Returns(new ShippingType("Express", 150m));
        _repositoryMock.Setup(r => r.GetByName("Dia siguiente")).Returns(new ShippingType("Dia siguiente", 80m));

        Assert.AreEqual(150m, _calculator.CalculateShippingCost("Express"));
        Assert.AreEqual(80m, _calculator.CalculateShippingCost("Dia siguiente"));
    }
}
