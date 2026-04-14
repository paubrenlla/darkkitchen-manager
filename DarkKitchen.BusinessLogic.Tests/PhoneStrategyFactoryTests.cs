using DarkKitchen.BusinessLogic.PhoneNumber;
using DarkKitchen.Domain.Users;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class PhoneStrategyFactoryTests
{
    [TestMethod]
    public void GetStrategy_ExistingPrefix_ReturnsCorrectStrategy()
    {
        var mockStrategyUy = new Mock<IPhoneValidationStrategy>();
        mockStrategyUy.Setup(s => s.CountryPrefix).Returns("+598");

        var mockStrategyAr = new Mock<IPhoneValidationStrategy>();
        mockStrategyAr.Setup(s => s.CountryPrefix).Returns("+54");

        var strategies = new List<IPhoneValidationStrategy>
        {
            mockStrategyUy.Object,
            mockStrategyAr.Object
        };

        var factory = new PhoneStrategyFactory(strategies);

        var result = factory.GetStrategy("+598");

        Assert.IsNotNull(result);
        Assert.AreEqual("+598", result.CountryPrefix);
        Assert.AreEqual(mockStrategyUy.Object, result);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void GetStrategy_NonExistingPrefix_ThrowsNotSupportedException()
    {
        var strategies = new List<IPhoneValidationStrategy>();
        var factory = new PhoneStrategyFactory(strategies);

        factory.GetStrategy("+1");
    }
}
