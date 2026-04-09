using System.Reflection;
using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.Tests;

[TestClass]
public class AuthControllerTests
{
    private const string ValidName = "Juan";
    private const string ValidSurname = "Perez";
    private const string ValidEmail = "test@domain.com";
    private const string ValidPassword = "Valid1Password!@";
    private static readonly IPhoneValidationStrategy UruguayStrategy = new UruguayPhoneValidationStrategy();
    private static readonly PhoneNumber ValidPhone = PhoneNumber.Create("+598", "094123456", UruguayStrategy);

    private AuthController _authController = null!;
    private Mock<IAuthService> _authServiceMock = null!;
    private Mock<IPhoneValidationStrategy> _phoneStrategyMock = null!;
    private Mock<ITokenService> _tokenServiceMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _authServiceMock = new Mock<IAuthService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _phoneStrategyMock = new Mock<IPhoneValidationStrategy>();
        _phoneStrategyMock.Setup(s => s.IsValid(It.IsAny<string>())).Returns(true);
        _authController = new AuthController(_authServiceMock.Object, _tokenServiceMock.Object);
    }

    [TestMethod]
    public void LoginSuccessful_ReturnsOkAndToken()
    {
        var request = new LoginRequest { Email = ValidEmail, Password = ValidPassword };
        var user = new User(ValidName, ValidSurname, request.Email, ValidPhone, request.Password, Role.Cliente);
        var generatedToken = "mocked.jwt.token";

        _authServiceMock.Setup(service => service.Login(request.Email, request.Password))
            .Returns(user);
        _tokenServiceMock.Setup(service => service.GenerateToken(user))
            .Returns(generatedToken);

        var result = _authController.Login(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        Type valueType = result.Value.GetType();
        PropertyInfo? tokenProperty = valueType.GetProperty("Token");
        Assert.IsNotNull(tokenProperty);

        var actualToken = tokenProperty.GetValue(result.Value)?.ToString();
        Assert.AreEqual(generatedToken, actualToken);

        _authServiceMock.Verify(service => service.Login(request.Email, request.Password), Times.Once);
        _tokenServiceMock.Verify(service => service.GenerateToken(user), Times.Once);
    }

    [TestMethod]
    public void LoginFailed_ReturnsUnauthorized()
    {
        var request = new LoginRequest { Email = ValidEmail, Password = "WrongPassword" };

        _authServiceMock.Setup(service => service.Login(request.Email, request.Password))
            .Throws(new UnauthorizedAccessException("Invalid Credentials."));

        var result = _authController.Login(request) as UnauthorizedObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(401, result.StatusCode);
    }

    [TestMethod]
    public void LoginSuccessful_ReturnsCorrectRole()
    {
        var request = new LoginRequest { Email = "admin@bmb.com", Password = "Valid1Password!@" };
        var user = new User("Juan", "Perez", request.Email, ValidPhone, request.Password, Role.Administrativo);

        _authServiceMock.Setup(service => service.Login(request.Email, request.Password))
            .Returns(user);

        var result = _authController.Login(request) as OkObjectResult;

        Assert.IsNotNull(result);

        Type valueType = result.Value.GetType();
        PropertyInfo? roleProperty = valueType.GetProperty("Role");
        Assert.IsNotNull(roleProperty);

        var actualRole = roleProperty.GetValue(result.Value)?.ToString();
        Assert.AreEqual("Administrativo", actualRole);
    }
}
