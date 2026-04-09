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
        new PhoneNumber("+598", invalidPhone, uruguayStrategy);
    }
}
