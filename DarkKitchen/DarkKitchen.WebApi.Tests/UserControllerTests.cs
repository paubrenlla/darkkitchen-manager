using System.Security.Claims;
using DarkKitchen.Domain.Users;
using DarkKitchen.Domain.Users.Encryptor;
using DarkKitchen.Domain.Users.PhoneValidations;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class UserControllerTests
{
    private Mock<IUserService> _userServiceMock = null!;
    private UserController _userController = null!;
    private Guid _callerId;

    [TestInitialize]
    public void Setup()
    {
        _userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
        _callerId = Guid.NewGuid();
        _userController = new UserController(_userServiceMock.Object);
        SetCallerContext(_callerId, "Administrativo");
    }

    private void SetCallerContext(Guid callerId, string role)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, callerId.ToString()),
            new Claim(ClaimTypes.Role, role),
        ];

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal },
        };
    }

    private static User CreateTestUser(string name = "Lucia", string surname = "Gomez", string email = "lucia@test.com", Role role = Role.Cliente)
    {
        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");
        var strategy = new UruguayPhoneValidationStrategy();
        var phone = Domain.Users.PhoneNumber.Create("+598", "094111222", strategy);
        return new User(name, surname, email, phone, "Valid1Password!@", role, hasher.Object);
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

        var user = CreateTestUser();
        _userServiceMock.Setup(s => s.CreateUser(request)).Returns(user);

        var result = _userController.CreateUser(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        var body = result.Value as UserCreateResponse;
        Assert.IsNotNull(body);
        Assert.AreEqual("Lucia", body.Name);
        Assert.AreEqual("lucia@test.com", body.Email);
        _userServiceMock.VerifyAll();
    }

    [TestMethod]
    public void GetUsers_ReturnsOkWithList()
    {
        var users = new List<User> { CreateTestUser("Juan", "Perez", "juan@test.com") };
        _userServiceMock.Setup(s => s.GetUsers("Juan", null)).Returns(users);

        var result = _userController.GetUsers("Juan", null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _userServiceMock.VerifyAll();
    }

    [TestMethod]
    public void GetUsers_NoResults_ReturnsNoContent()
    {
        _userServiceMock.Setup(s => s.GetUsers(It.IsAny<string>(), It.IsAny<string?>()))
            .Returns([]);

        var result = _userController.GetUsers("Inexistente", null) as NoContentResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(204, result.StatusCode);
        _userServiceMock.VerifyAll();
    }

    [TestMethod]
    public void UpdateUser_ValidRequest_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var request = new UserUpdateRequest
        {
            Name = "Nuevo",
            Surname = "Nombre",
            Email = "nuevo@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094999888",
            Role = "Administrativo",
        };

        var user = CreateTestUser("Nuevo", "Nombre", "nuevo@test.com", Role.Administrativo);
        _userServiceMock.Setup(s => s.UpdateUser(_callerId, userId, request)).Returns(user);

        var result = _userController.UpdateUser(userId, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _userServiceMock.VerifyAll();
    }

    [TestMethod]
    public void DeleteUser_ValidRequest_ReturnsNoContent()
    {
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(s => s.DeleteUser(_callerId, userId));

        var result = _userController.DeleteUser(userId) as NoContentResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(204, result.StatusCode);
        _userServiceMock.VerifyAll();
    }

    [TestMethod]
    public void CreateUser_PreparadorCreatesWithRole_ReturnsForbid()
    {
        SetCallerContext(_callerId, "Preparador");
        var request = new UserCreateRequest
        {
            Name = "Carlos",
            Surname = "Lopez",
            Email = "carlos@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094111333",
            Password = "Valid1Password!@",
            Role = "Preparador",
        };

        var result = _userController.CreateUser(request) as ForbidResult;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void CreateUser_ClienteCreatesWithRole_ReturnsForbid()
    {
        SetCallerContext(_callerId, "Cliente");
        var request = new UserCreateRequest
        {
            Name = "Carlos",
            Surname = "Lopez",
            Email = "carlos@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094111333",
            Password = "Valid1Password!@",
            Role = "Preparador",
        };

        var result = _userController.CreateUser(request) as ForbidResult;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void CreateUser_AnonymousWithRole_ReturnsForbid()
    {
        _userController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var request = new UserCreateRequest
        {
            Name = "N",
            Surname = "S",
            Email = "t@t.com",
            Password = "Valid1Password!@",
            Role = "Cliente",
            CountryPrefix = "+598",
            PhoneNumber = "0941",
        };

        var result = _userController.CreateUser(request) as ForbidResult;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void CreateUser_WithNoRoleInRequest_ReturnsCreated()
    {
        SetCallerContext(_callerId, "Cualquiera");
        var request = new UserCreateRequest
        {
            Name = "N",
            Surname = "S",
            Email = "t@t.com",
            Password = "Valid1Password!@",
            Role = null,
            CountryPrefix = "+598",
            PhoneNumber = "0941",
        };

        var user = CreateTestUser("Nolan", "Savhrina", "t@t.com");
        _userServiceMock.Setup(s => s.CreateUser(request)).Returns(user);

        var result = _userController.CreateUser(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        _userServiceMock.VerifyAll();
    }

    [TestMethod]
    public void GetUsers_ShouldHaveAuthorizeAttributeWithAdministrativoRole()
    {
        var method = typeof(UserController).GetMethod(nameof(UserController.GetUsers));
        var attribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .FirstOrDefault() as AuthorizeAttribute;

        Assert.IsNotNull(attribute);
        Assert.AreEqual("Administrativo", attribute.Roles);
    }

    [TestMethod]
    public void UpdateUser_ShouldHaveAuthorizeAttributeWithAdministrativoRole()
    {
        var method = typeof(UserController).GetMethod(nameof(UserController.UpdateUser), [typeof(Guid), typeof(UserUpdateRequest)]);
        var attribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .FirstOrDefault() as AuthorizeAttribute;

        Assert.IsNotNull(attribute);
        Assert.AreEqual("Administrativo", attribute.Roles);
    }

    [TestMethod]
    public void DeleteUser_ShouldHaveAuthorizeAttributeWithAdministrativoRole()
    {
        var method = typeof(UserController).GetMethod(nameof(UserController.DeleteUser));
        var attribute = method?.GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .FirstOrDefault() as AuthorizeAttribute;

        Assert.IsNotNull(attribute);
        Assert.AreEqual("Administrativo", attribute.Roles);
    }
}
