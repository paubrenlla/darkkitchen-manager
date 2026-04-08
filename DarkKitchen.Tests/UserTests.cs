using DarkKitchen.Domain.Users;

namespace DarkKitchen.Tests;

[TestClass]
public class UserTests
{
    private const string ValidName = "Juan";
    private const string ValidSurname = "Perez";
    private const string ValidEmail = "test@domain.com";
    private const string ValidPhone = "094123456";
    private const string ValidPassword = "Valid1Password!@";

    private readonly IPhoneValidationStrategy _strategy = new UruguayPhoneValidationStrategy();

    [TestMethod]
    public void CreateUser_WithValidData_CreatesSuccessfully()
    {
        var user = new User(ValidName, ValidSurname, ValidEmail, ValidPhone, ValidPassword, _strategy);

        Assert.IsNotNull(user);
        Assert.AreEqual(ValidName, user.Name);
        Assert.AreEqual(Role.Cliente, user.Role);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Name must have at least 1 character.")]
    public void CreateUser_WithEmptyName_ThrowsArgumentException()
    {
        var name = string.Empty;
        new User(name, ValidSurname, ValidEmail, ValidPhone, ValidPassword, _strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Name must have at least 1 character.")]
    public void CreateUser_WithNullName_ThrowsArgumentException()
    {
        string name = null!;
        new User(name, ValidSurname, ValidEmail, ValidPhone, ValidPassword, _strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Surname must be between 3 and 25 characters.")]
    public void CreateUser_WithShortSurname_ThrowsArgumentException()
    {
        var shortSurname = "Al";
        new User("Juan", shortSurname, ValidEmail, ValidPhone, ValidPassword, _strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Surname must be between 3 and 25 characters.")]
    public void CreateUser_WithLongSurname_ThrowsArgumentException()
    {
        var longSurname = new string('a', 26);
        new User(ValidName, longSurname, ValidEmail, ValidPhone, ValidPassword, _strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Invalid email format.")]
    public void CreateUser_WithEmptyEmail_ThrowsArgumentException()
    {
        var email = string.Empty;
        new User(ValidName, ValidSurname, email, ValidPhone, ValidPassword, _strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Invalid email format.")]
    public void CreateUser_WithInvalidEmailFormat_ThrowsArgumentException()
    {
        var email = "invalid.email.com";
        new User(ValidName, ValidSurname, email, ValidPhone, ValidPassword, _strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password must be between 15 and 25 characters.")]
    public void CreateUser_WithShortPassword_ThrowsArgumentException()
    {
        var shortPassword = "Short1!pass";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, shortPassword, _strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password must be between 15 and 25 characters.")]
    public void CreateUser_WithLongPassword_ThrowsArgumentException()
    {
        var longPassword = "Valid1Password!@1234567890ABC";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, longPassword, _strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password must contain at least one uppercase letter.")]
    public void CreateUser_PasswordWithoutUppercase_ThrowsArgumentException()
    {
        var noUpper = "valid1password!@";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, noUpper, _strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password must contain at least one lowercase letter.")]
    public void CreateUser_PasswordWithoutLowercase_ThrowsArgumentException()
    {
        var noLower = "VALID1PASSWORD!@";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, noLower, _strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password must contain at least one number.")]
    public void CreateUser_PasswordWithoutNumber_ThrowsArgumentException()
    {
        var noNumber = "ValidPassword!@#";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, noNumber, _strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password must contain at least one symbol.")]
    public void CreateUser_PasswordWithoutSymbol_ThrowsArgumentException()
    {
        var noSymbol = "Valid1PasswordAA";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, noSymbol, _strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password cannot contain sequences.")]
    public void CreateUser_PasswordWithSequence_ThrowsArgumentException()
    {
        var withSequence = "Valid1Password!@123";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, withSequence, _strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Invalid phone format.")]
    public void CreateUser_InvalidUruguayPhone_ThrowsArgumentException()
    {
        var strategy = new UruguayPhoneValidationStrategy();
        var invalidPhone = "123";

        new User(ValidName, ValidSurname, ValidEmail, invalidPhone, ValidPassword, Role.Cliente, strategy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CreateUser_NullStrategy_ThrowsArgumentNullException()
    {
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, ValidPassword, null!);
    }

    [TestMethod]
    public void CreateUser_ShouldStorePhoneWithCountryPrefix()
    {
        var strategy = new UruguayPhoneValidationStrategy();
        var inputPhone = "094123456";
        var expectedPhone = "+598094123456";

        var user = new User(ValidName, ValidSurname, ValidEmail, inputPhone, ValidPassword, strategy);

        Assert.AreEqual(expectedPhone, user.Phone);
    }

    [TestMethod]
    public void CreateUser_WithSpacesAndDashes_ShouldStoreNormalizedPhone()
    {
        var strategy = new UruguayPhoneValidationStrategy();
        var inputPhone = "094-123 456";
        var expected = "+598094123456";

        var user = new User(ValidName, ValidSurname, ValidEmail, inputPhone, ValidPassword, strategy);

        Assert.AreEqual(expected, user.Phone);
    }
}
