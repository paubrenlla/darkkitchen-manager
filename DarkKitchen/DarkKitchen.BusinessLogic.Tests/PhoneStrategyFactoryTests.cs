using DarkKitchen.BusinessLogic.PhoneNumber;
using DarkKitchen.Domain.Users.PhoneValidations;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class PhoneStrategyFactoryTests
{
    [TestMethod]
    public void GetStrategy_ExistingPrefix_ReturnsCorrectStrategy()
    {
        var mockStrategyUy = new Mock<IPhoneValidationStrategy>(MockBehavior.Strict);
        mockStrategyUy.Setup(s => s.CountryPrefix).Returns("+598");

        var mockStrategyAr = new Mock<IPhoneValidationStrategy>(MockBehavior.Strict);

        var factory = new PhoneStrategyFactory([mockStrategyUy.Object, mockStrategyAr.Object]);

        var result = factory.GetStrategy("+598");

        Assert.IsNotNull(result);
        Assert.AreEqual("+598", result.CountryPrefix);
        Assert.AreEqual(mockStrategyUy.Object, result);
        mockStrategyUy.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void GetStrategy_NonExistingPrefix_ThrowsNotSupportedException()
    {
        var factory = new PhoneStrategyFactory([]);

        factory.GetStrategy("+1");
    }
}
