using DarkKitchen.Domain.Users;

namespace DarkKitchen.Tests;

[TestClass]
public class PhoneNumberTests
{
    private static readonly IPhoneValidationStrategy uruguayStrategy = new UruguayPhoneValidationStrategy();

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Invalid phone format.")]
    public void CreateUser_InvalidUruguayPhone_ThrowsArgumentException()
    {
        var invalidPhone = "123";
        PhoneNumber.Create("+598", invalidPhone, uruguayStrategy);
    }

    [TestMethod]
    public void CreatePhone_WithSpacesAndDashes_IsNormalizedAndSucceeds()
    {
        var rawPhone = "094-123 456";
        var expectedPhone = "094123456";

        var phone = PhoneNumber.Create(rawPhone, "+598", uruguayStrategy);

        Assert.AreEqual(expectedPhone, phone.Number);
    }
}
