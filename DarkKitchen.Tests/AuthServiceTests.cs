using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.Domain.Users;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.Tests;

[TestClass]
public class AuthServiceTests
{
    private const string ValidName = "Juan";
    private const string ValidSurname = "Perez";
    private const string ValidEmail = "test@domain.com";
    private const string ValidPassword = "Valid1Password!@";
    private static readonly IPhoneValidationStrategy uruguayStrategy = new UruguayPhoneValidationStrategy();
    private static readonly PhoneNumber ValidPhone = PhoneNumber.Create("+598", "094123456", uruguayStrategy);

    private AuthService _authService = null!;
    private Mock<IPhoneValidationStrategy> _phoneStrategyMock = null!;
    private Mock<IUserRepository> _userRepositoryMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _phoneStrategyMock = new Mock<IPhoneValidationStrategy>();
        _phoneStrategyMock.Setup(s => s.IsValid("+598", It.IsAny<string>())).Returns(true);
        _authService = new AuthService(_userRepositoryMock.Object);
    }

    [TestMethod]
    public void LoginWithValidCredentials_ReturnsUser()
    {
        var expectedUser = new User(ValidName, ValidSurname, ValidEmail, ValidPhone, ValidPassword, Role.Cliente);

        _userRepositoryMock.Setup(repo => repo.GetUserByEmail(ValidEmail))
            .Returns(expectedUser);

        User result = _authService.Login(ValidEmail, ValidPassword);

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedUser.Id, result.Id);
        Assert.AreEqual(expectedUser.Email, result.Email);
        Assert.AreEqual(expectedUser.Role, result.Role);
        _userRepositoryMock.Verify(repo => repo.GetUserByEmail(ValidEmail), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void LoginWithInvalidPassword_ThrowsUnauthorized()
    {
        var invalidPassword = "WrongPassword";
        var existingUser = new User(ValidName, ValidSurname, ValidEmail, ValidPhone, ValidPassword, Role.Cliente);

        _userRepositoryMock.Setup(repo => repo.GetUserByEmail(ValidEmail))
            .Returns(existingUser);

        _authService.Login(ValidEmail, invalidPassword);
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void LoginWithNonExistentEmail_ThrowsUnauthorized()
    {
        var email = "nonexistent@bmb.com";

        _userRepositoryMock.Setup(repo => repo.GetUserByEmail(email))
            .Returns((User)null!);

        _authService.Login(email, ValidPassword);
    }
}
