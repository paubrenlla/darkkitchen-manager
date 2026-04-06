using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.Tests;

[TestClass]
public class AuthControllerTests
{
    private Mock<IAuthService> _authServiceMock = null!;
    private AuthController _authController = null!;

    [TestInitialize]
    public void Setup()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
    }

    [TestMethod]
    public void LoginSuccessful_ReturnsOk()
    {
        var request = new LoginRequest("test@bmb.com", "Password123!");
        var user = new User { Id = Guid.NewGuid(), Email = request.Email, Password = request.Password, Role = Role.Cliente };

        _authServiceMock.Setup(service => service.Login(request.Email, request.Password))
                        .Returns(user);

        var result = _authController.Login(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _authServiceMock.Verify(service => service.Login(request.Email, request.Password), Times.Once);
    }

    [TestMethod]
    public void LoginFailed_ReturnsUnauthorized()
    {
        var request = new LoginRequest("test@bmb.com", "WrongPassword");

        _authServiceMock.Setup(service => service.Login(request.Email, request.Password))
                        .Throws(new UnauthorizedAccessException("Credenciales inválidas."));

        var result = _authController.Login(request) as UnauthorizedObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(401, result.StatusCode);
    }

    [TestMethod]
    public void LoginSuccessful_ReturnsCorrectRole()
    {
        var request = new LoginRequest("admin@bmb.com", "Password123!");
        var user = new User { Id = Guid.NewGuid(), Email = request.Email, Password = request.Password, Role = Role.Administrativo };

        _authServiceMock.Setup(service => service.Login(request.Email, request.Password))
                        .Returns(user);

        var result = _authController.Login(request) as OkObjectResult;

        Assert.IsNotNull(result);

        var valueType = result.Value.GetType();
        var roleProperty = valueType.GetProperty("Role");
        Assert.IsNotNull(roleProperty);

        var actualRole = roleProperty.GetValue(result.Value)?.ToString();
        Assert.AreEqual("Administrativo", actualRole);
    }
}
