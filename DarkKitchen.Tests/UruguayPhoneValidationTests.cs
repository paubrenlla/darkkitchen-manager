using DarkKitchen.Domain.Users;

namespace DarkKitchen.Tests;

[TestClass]
public class UruguayPhoneValidationTests
{
    [TestMethod]
    public void IsValid_UruguayMobile_ReturnsTrue()
    {
        var strategy = new UruguayPhoneValidationStrategy();
        var phone = "094123456";

        var res = strategy.IsValid(phone);

        Assert.IsTrue(res);
    }

    [TestMethod]
    public void IsValid_InvalidLength_ReturnsFalse()
    {
        var strategy = new UruguayPhoneValidationStrategy();
        var phone = "094123";

        var res = strategy.IsValid(phone);

        Assert.IsFalse(res);
    }

    [TestMethod]
    public void IsValid_NumberTooLong_ReturnsFalse()
    {
        var strategy = new UruguayPhoneValidationStrategy();
        var phone = "094123123123";

        var res = strategy.IsValid(phone);

        Assert.IsFalse(res);
    }

    [TestMethod]
    public void IsValid_DoesNotStartWith09_ReturnsFalse()
    {
        var strategy = new UruguayPhoneValidationStrategy();
        var result = strategy.IsValid("123456789");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_NullPhone_ReturnsFalse()
    {
        var strategy = new UruguayPhoneValidationStrategy();

        var res = strategy.IsValid(null!);

        Assert.IsFalse(res);
    }

    [TestMethod]
    public void IsValid_EmptyPhone_ReturnsFalse()
    {
        var strategy = new UruguayPhoneValidationStrategy();

        var res = strategy.IsValid(string.Empty);

        Assert.IsFalse(res);
    }

    [TestMethod]
    public void IsValid_ContainsLetters_ReturnsFalse()
    {
        var strategy = new UruguayPhoneValidationStrategy();

        var res = strategy.IsValid("099ABC123");

        Assert.IsFalse(res, "Phone number cannot contain letters.");
    }
}
