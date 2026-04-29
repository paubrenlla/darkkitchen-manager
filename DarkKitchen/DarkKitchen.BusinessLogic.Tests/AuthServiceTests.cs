using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class AuthServiceTests
{
    private const string ValidName = "Juan";
    private const string ValidSurname = "Perez";
    private const string ValidEmail = "test@domain.com";
    private const string ValidPassword = "Valid1Password!@";
    private static readonly IPhoneValidationStrategy UruguayStrategy = new UruguayPhoneValidationStrategy();
    private static readonly Domain.Users.PhoneNumber ValidPhone = Domain.Users.PhoneNumber.Create("+598", "094123456", UruguayStrategy);

    private AuthService _authService = null!;
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<ITokenService> _tokenServiceMock = null!;
    private Mock<IPasswordHasher> _passwordHasherMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _authService = new AuthService(
            _userRepositoryMock.Object,
            _tokenServiceMock.Object,
            _passwordHasherMock.Object);
    }

    [TestMethod]
    public void LoginWithValidCredentials_ReturnsLoginResponse()
    {
        var expectedUser = new User(ValidName, ValidSurname, ValidEmail, ValidPhone, ValidPassword, Role.Cliente, _passwordHasherMock.Object);

        _userRepositoryMock.Setup(repo => repo.GetUserByEmail(ValidEmail)).Returns(expectedUser);
        _passwordHasherMock.Setup(h => h.VerifyPassword(ValidPassword, expectedUser.HashedPassword)).Returns(true);
        _tokenServiceMock.Setup(t => t.GenerateToken(expectedUser)).Returns("mocked.token");

        var result = _authService.Login(ValidEmail, ValidPassword);

        Assert.IsNotNull(result);
        Assert.AreEqual("mocked.token", result.Token);
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void LoginWithInvalidPassword_ThrowsUnauthorized()
    {
        var existingUser = new User(ValidName, ValidSurname, ValidEmail, ValidPhone, ValidPassword, Role.Cliente, _passwordHasherMock.Object);

        _userRepositoryMock.Setup(repo => repo.GetUserByEmail(ValidEmail)).Returns(existingUser);
        _passwordHasherMock.Setup(h => h.VerifyPassword("WrongPassword", existingUser.HashedPassword)).Returns(false);

        _authService.Login(ValidEmail, "WrongPassword");
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void LoginWithNonExistentEmail_ThrowsUnauthorized()
    {
        _userRepositoryMock.Setup(repo => repo.GetUserByEmail("nonexistent@bmb.com")).Returns((User?)null);

        _authService.Login("nonexistent@bmb.com", ValidPassword);
    }
}
