using DarkKitchen.Domain.Users;
using DarkKitchen.Domain.Users.PhoneValidations;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class PhoneNumberTests
{
    private static readonly IPhoneValidationStrategy UruguayStrategy = new UruguayPhoneValidationStrategy();

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Invalid phone format.")]
    public void CreateUser_InvalidUruguayPhone_ThrowsArgumentException()
    {
        var invalidPhone = "123";
        PhoneNumber.Create("+598", invalidPhone, UruguayStrategy);
    }

    [TestMethod]
    public void CreatePhone_WithSpacesAndDashes_IsNormalizedAndSucceeds()
    {
        var rawPhone = "094-123 456";
        var expectedPhone = "094123456";

        var phone = PhoneNumber.Create("+598", rawPhone, UruguayStrategy);

        Assert.AreEqual(expectedPhone, phone.Number);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CreatePhone_NullStrategy_ThrowsArgumentException()
    {
        PhoneNumber.Create("+598", "094123456", null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePhone_NullNumber_ThrowsArgumentException()
    {
        PhoneNumber.Create("+598", null!, UruguayStrategy);
    }
}
