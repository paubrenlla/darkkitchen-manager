using DarkKitchen.Domain.Users;
using Moq;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class UserTests
{
    private const string ValidName = "Juan";
    private const string ValidSurname = "Perez";
    private const string ValidEmail = "test@domain.com";
    private const string ValidPassword = "Valid1Password!@";
    private static readonly IPhoneValidationStrategy UruguayStrategy = new UruguayPhoneValidationStrategy();
    private static readonly PhoneNumber ValidPhone = PhoneNumber.Create("+598", "094123456", UruguayStrategy);
    private IPasswordHasher _passwordHasher = null!;

    [TestInitialize]
    public void Setup()
    {
        var mock = new Mock<IPasswordHasher>();
        mock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");
        _passwordHasher = mock.Object;
    }

    [TestMethod]
    public void CreateUser_WithValidData_CreatesSuccessfully()
    {
        var user = new User(ValidName, ValidSurname, ValidEmail, ValidPhone, ValidPassword,_passwordHasher);

        Assert.IsNotNull(user);
        Assert.AreEqual(ValidName, user.Name);
        Assert.AreEqual(Role.Cliente, user.Role);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Name must have at least 1 character.")]
    public void CreateUser_WithEmptyName_ThrowsArgumentException()
    {
        var name = string.Empty;
        new User(name, ValidSurname, ValidEmail, ValidPhone, ValidPassword,_passwordHasher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Name must have at least 1 character.")]
    public void CreateUser_WithNullName_ThrowsArgumentException()
    {
        string name = null!;
        new User(name, ValidSurname, ValidEmail, ValidPhone, ValidPassword,_passwordHasher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Surname must be between 3 and 25 characters.")]
    public void CreateUser_WithShortSurname_ThrowsArgumentException()
    {
        var shortSurname = "Al";
        new User("Juan", shortSurname, ValidEmail, ValidPhone, ValidPassword,_passwordHasher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Surname must be between 3 and 25 characters.")]
    public void CreateUser_WithLongSurname_ThrowsArgumentException()
    {
        var longSurname = new string('a', 26);
        new User(ValidName, longSurname, ValidEmail, ValidPhone, ValidPassword,_passwordHasher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Invalid email format.")]
    public void CreateUser_WithEmptyEmail_ThrowsArgumentException()
    {
        var email = string.Empty;
        new User(ValidName, ValidSurname, email, ValidPhone, ValidPassword,_passwordHasher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Invalid email format.")]
    public void CreateUser_WithInvalidEmailFormat_ThrowsArgumentException()
    {
        var email = "invalid.email.com";
        new User(ValidName, ValidSurname, email, ValidPhone, ValidPassword, _passwordHasher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password must be between 15 and 25 characters.")]
    public void CreateUser_WithShortPassword_ThrowsArgumentException()
    {
        var shortPassword = "Short1!pass";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, shortPassword, _passwordHasher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password must be between 15 and 25 characters.")]
    public void CreateUser_WithLongPassword_ThrowsArgumentException()
    {
        var longPassword = "Valid1Password!@1234567890ABC";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, longPassword, _passwordHasher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password must contain at least one uppercase letter.")]
    public void CreateUser_PasswordWithoutUppercase_ThrowsArgumentException()
    {
        var noUpper = "valid1password!@";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, noUpper, _passwordHasher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password must contain at least one lowercase letter.")]
    public void CreateUser_PasswordWithoutLowercase_ThrowsArgumentException()
    {
        var noLower = "VALID1PASSWORD!@";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, noLower, _passwordHasher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password must contain at least one number.")]
    public void CreateUser_PasswordWithoutNumber_ThrowsArgumentException()
    {
        var noNumber = "ValidPassword!@#";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, noNumber, _passwordHasher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password must contain at least one symbol.")]
    public void CreateUser_PasswordWithoutSymbol_ThrowsArgumentException()
    {
        var noSymbol = "Valid1PasswordAA";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, noSymbol, _passwordHasher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password cannot contain sequences.")]
    public void CreateUser_PasswordWithSequence_ThrowsArgumentException()
    {
        var withSequence = "Valid1Password!@123";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, withSequence, _passwordHasher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Password cannot contain sequences.")]
    public void CreateUser_PasswordWithDescendingSequence_ThrowsArgumentException()
    {
        var withDescendingSequence = "Valid1Password!@321";
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, withDescendingSequence, _passwordHasher);
    }

    [TestMethod]
    public void UpdateDetails_ShouldUpdateFieldsAndPreserveId()
    {
        IPhoneValidationStrategy strategy = new UruguayPhoneValidationStrategy();
        var phone = PhoneNumber.Create("+598", "094123456", strategy);
        var user = new User("Juan", "Perez", "juan@test.com", phone, "Valid1Password!@", Role.Cliente, _passwordHasher);
        Guid originalId = user.Id;

        var newPhone = PhoneNumber.Create("+598", "094999888", strategy);
        user.UpdateDetails("Nuevo", "Nombre", "nuevo@test.com", newPhone, Role.Administrativo);

        Assert.AreEqual(originalId, user.Id);
        Assert.AreEqual("Nuevo", user.Name);
        Assert.AreEqual("Nombre", user.Surname);
        Assert.AreEqual("nuevo@test.com", user.Email);
        Assert.AreEqual(Role.Administrativo, user.Role);
    }
}
