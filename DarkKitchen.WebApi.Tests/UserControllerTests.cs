using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class UserControllerTests
{
    private Mock<IUserService> _userServiceMock = null!;
    private UserController _userController = null!;

    [TestInitialize]
    public void Setup()
    {
        _userServiceMock = new Mock<IUserService>();
        _userController = new UserController(_userServiceMock.Object);
    }

    [TestMethod]
    public void CreateUser_Success_ReturnsCreatedAndUserData()
    {
        var request = new UserCreateRequest
        {
            Name = "Lucia",
            Surname = "Gomez",
            Email = "lucia@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094111222",
            Password = "Valid1Password!@",
        };

        var mockPhone = PhoneNumber.Create("+598", "094111222", new UruguayPhoneValidationStrategy());
        var createdUser = new User(request.Name, request.Surname, request.Email, mockPhone, request.Password, Role.Cliente);

        _userServiceMock.Setup(s => s.CreateUser(request)).Returns(createdUser);

        var result = _userController.CreateUser(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        Assert.IsNotNull(result.Value);
    }

    [TestMethod]
    public void CreateUser_ServiceThrowsArgumentException_ReturnsBadRequest()
    {
        var request = new UserCreateRequest
        {
            Name = string.Empty,
            Surname = "Gomez",
            Email = "lucia@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094111222",
            Password = "Valid1Password!@"
        };

        _userServiceMock.Setup(s => s.CreateUser(request))
            .Throws(new ArgumentException("Nombre inválido."));

        var result = _userController.CreateUser(request) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }
}
