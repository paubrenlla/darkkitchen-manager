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
        new PhoneNumber(invalidPhone, uruguayStrategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CreateUser_NullStrategy_ThrowsArgumentNullException()
    {
        new PhoneNumber("094123456", null!);
    }
}
