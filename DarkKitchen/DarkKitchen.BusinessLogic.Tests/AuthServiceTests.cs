using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.Domain.Users;
using DarkKitchen.Domain.Users.Encryptor;
using DarkKitchen.Domain.Users.PhoneValidations;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class AuthServiceTests
{
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
        _userRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
        _tokenServiceMock = new Mock<ITokenService>(MockBehavior.Strict);
        _passwordHasherMock = new Mock<IPasswordHasher>(MockBehavior.Strict);
        _authService = new AuthService(
            _userRepositoryMock.Object,
            _tokenServiceMock.Object,
            _passwordHasherMock.Object);
    }

    private User CreateTestUser()
    {
        _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");
        return new User("Juan", "Perez", ValidEmail, ValidPhone, ValidPassword, Role.Cliente, _passwordHasherMock.Object);
    }

    [TestMethod]
    public void LoginWithValidCredentials_ReturnsLoginResponse()
    {
        var user = CreateTestUser();
        _userRepositoryMock.Setup(repo => repo.GetUserByEmail(ValidEmail)).Returns(user);
        _passwordHasherMock.Setup(h => h.VerifyPassword(ValidPassword, user.HashedPassword)).Returns(true);
        _tokenServiceMock.Setup(t => t.GenerateToken(user)).Returns("mocked.token");

        var result = _authService.Login(ValidEmail, ValidPassword);

        Assert.IsNotNull(result);
        Assert.AreEqual("mocked.token", result.Token);
        _userRepositoryMock.VerifyAll();
        _passwordHasherMock.VerifyAll();
        _tokenServiceMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void LoginWithInvalidPassword_ThrowsUnauthorized()
    {
        var user = CreateTestUser();
        _userRepositoryMock.Setup(repo => repo.GetUserByEmail(ValidEmail)).Returns(user);
        _passwordHasherMock.Setup(h => h.VerifyPassword("WrongPassword", user.HashedPassword)).Returns(false);

        _authService.Login(ValidEmail, "WrongPassword");

        _userRepositoryMock.VerifyAll();
        _passwordHasherMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void LoginWithNonExistentEmail_ThrowsUnauthorized()
    {
        _userRepositoryMock.Setup(repo => repo.GetUserByEmail("nonexistent@bmb.com")).Returns((User?)null);

        _authService.Login("nonexistent@bmb.com", ValidPassword);

        _userRepositoryMock.VerifyAll();
    }
}
