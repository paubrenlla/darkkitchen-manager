using DarkKitchen.Domain.Users;
using DarkKitchen.Domain.Users.Encryptor;
using DarkKitchen.Domain.Users.PhoneValidations;
using DarkKitchen.IBusinessLogic.IPhoneNumber;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IPhoneStrategyFactory> _strategyFactoryMock = null!;
    private UserService _userService = null!;
    private Mock<IPasswordHasher> _passwordHasherMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
        _strategyFactoryMock = new Mock<IPhoneStrategyFactory>(MockBehavior.Strict);
        _passwordHasherMock = new Mock<IPasswordHasher>(MockBehavior.Strict);
        _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed_password");
        _userService = new UserService(
            _userRepositoryMock.Object,
            _strategyFactoryMock.Object,
            _passwordHasherMock.Object);
    }

    private Mock<IPhoneValidationStrategy> SetupPhoneStrategy(string prefix = "+598", string number = "094123456")
    {
        var mockStrategy = new Mock<IPhoneValidationStrategy>(MockBehavior.Strict);
        mockStrategy.Setup(s => s.IsValid(number)).Returns(true);
        _strategyFactoryMock.Setup(f => f.GetStrategy(prefix)).Returns(mockStrategy.Object);
        return mockStrategy;
    }

    private User CreateTestUser(string name = "Juan", string surname = "Perez", string email = "juan@test.com", Role role = Role.Cliente)
    {
        IPhoneValidationStrategy strategy = new UruguayPhoneValidationStrategy();
        var phone = Domain.Users.PhoneNumber.Create("+598", "094123456", strategy);
        return new User(name, surname, email, phone, "Valid1Password!@", role, _passwordHasherMock.Object);
    }

    [TestMethod]
    public void CreateUser_ValidRequest_CreatesUserAndCallsRepository()
    {
        SetupPhoneStrategy();
        _userRepositoryMock.Setup(r => r.GetUserByEmail("carlos@test.com")).Returns((User?)null);
        _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>()));

        var request = new UserCreateRequest
        {
            Name = "Carlos",
            Surname = "Perez",
            Email = "carlos@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094123456",
            Password = "Valid1Password!@"
        };

        var result = _userService.CreateUser(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Carlos", result.Name);
        _userRepositoryMock.VerifyAll();
        _strategyFactoryMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void CreateUser_InvalidPhoneStrategy_ThrowsException()
    {
        var request = new UserCreateRequest
        {
            Name = "Carlos",
            Surname = "Perez",
            Email = "carlos@test.com",
            CountryPrefix = "+123",
            PhoneNumber = "12345678",
            Password = "Valid1Password!@"
        };

        _userRepositoryMock.Setup(r => r.GetUserByEmail("carlos@test.com")).Returns((User?)null);
        _strategyFactoryMock.Setup(f => f.GetStrategy("+123"))
            .Throws(new NotSupportedException("Country prefix not supported."));

        _userService.CreateUser(request);

        _strategyFactoryMock.VerifyAll();
    }

    [TestMethod]
    public void CreateUser_WithExplicitRole_ShouldUseProvidedRole()
    {
        SetupPhoneStrategy();
        _userRepositoryMock.Setup(r => r.GetUserByEmail("carlos@test.com")).Returns((User?)null);
        _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>()));

        var request = new UserCreateRequest
        {
            Name = "Carlos",
            Surname = "Perez",
            Email = "carlos@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094123456",
            Password = "Valid1Password!@",
            Role = "Administrativo",
        };

        var result = _userService.CreateUser(request);

        Assert.IsNotNull(result);
        _userRepositoryMock.VerifyAll();
        _strategyFactoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetUsers_ShouldDelegateToRepository()
    {
        var users = new List<User> { CreateTestUser() };
        _userRepositoryMock.Setup(r => r.GetByNameAndSurname("Juan", "Perez")).Returns(users);

        var result = _userService.GetUsers("Juan", "Perez").ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Juan", result[0].Name);
        _userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void UpdateUser_ValidRequest_ShouldUpdateAndReturn()
    {
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingUser = CreateTestUser("Old", "Name", "old@test.com", Role.Preparador);

        _userRepositoryMock.Setup(r => r.GetById(userId)).Returns(existingUser);
        _userRepositoryMock.Setup(r => r.GetUserByEmail("nuevo@test.com")).Returns((User?)null);
        _userRepositoryMock.Setup(r => r.Update(userId, It.IsAny<User>()));
        SetupPhoneStrategy("+598", "094999888");

        var request = new UserUpdateRequest
        {
            Name = "Nuevo",
            Surname = "Nombre",
            Email = "nuevo@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094999888",
            Role = "Administrativo",
        };

        var result = _userService.UpdateUser(adminId, userId, request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Nuevo", result.Name);
        _userRepositoryMock.VerifyAll();
        _strategyFactoryMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void UpdateUser_SelfModification_ShouldThrow()
    {
        var adminId = Guid.NewGuid();

        var request = new UserUpdateRequest
        {
            Name = "Nuevo",
            Surname = "Nombre",
            Email = "nuevo@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094999888",
            Role = "Administrativo",
        };

        _userService.UpdateUser(adminId, adminId, request);
    }

    [TestMethod]
    public void DeleteUser_ValidRequest_ShouldCallRepositoryDelete()
    {
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingUser = CreateTestUser();

        _userRepositoryMock.Setup(r => r.GetById(userId)).Returns(existingUser);
        _userRepositoryMock.Setup(r => r.Delete(userId));

        _userService.DeleteUser(adminId, userId);

        _userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DeleteUser_SelfDeletion_ShouldThrow()
    {
        var adminId = Guid.NewGuid();

        _userService.DeleteUser(adminId, adminId);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WithClienteRole_ShouldThrow()
    {
        var request = new UserCreateRequest
        {
            Name = "Juan",
            Surname = "Perez",
            Email = "juan@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094123456",
            Password = "Valid1Password!@",
            Role = "Cliente",
        };

        _userService.CreateUser(request);
    }

    [TestMethod]
    public void CreateUser_WithAdministrativoRole_ShouldSucceed()
    {
        SetupPhoneStrategy();
        _userRepositoryMock.Setup(r => r.GetUserByEmail("juan@test.com")).Returns((User?)null);
        _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>()));

        var request = new UserCreateRequest
        {
            Name = "Juan",
            Surname = "Perez",
            Email = "juan@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094123456",
            Password = "Valid1Password!@",
            Role = "Administrativo",
        };

        var result = _userService.CreateUser(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(Role.Administrativo, result.Role);
        _userRepositoryMock.VerifyAll();
        _strategyFactoryMock.VerifyAll();
    }

    [TestMethod]
    public void CreateUser_WithPreparadorRole_ShouldSucceed()
    {
        SetupPhoneStrategy();
        _userRepositoryMock.Setup(r => r.GetUserByEmail("juan@test.com")).Returns((User?)null);
        _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>()));

        var request = new UserCreateRequest
        {
            Name = "Juan",
            Surname = "Perez",
            Email = "juan@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094123456",
            Password = "Valid1Password!@",
            Role = "Preparador",
        };

        var result = _userService.CreateUser(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(Role.Preparador, result.Role);
        _userRepositoryMock.VerifyAll();
        _strategyFactoryMock.VerifyAll();
    }

    [TestMethod]
    public void CreateUser_WithNullRole_ShouldCreateCliente()
    {
        SetupPhoneStrategy();
        _userRepositoryMock.Setup(r => r.GetUserByEmail("juan@test.com")).Returns((User?)null);
        _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>()));

        var request = new UserCreateRequest
        {
            Name = "Juan",
            Surname = "Perez",
            Email = "juan@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094123456",
            Password = "Valid1Password!@",
            Role = null,
        };

        var result = _userService.CreateUser(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(Role.Cliente, result.Role);
        _userRepositoryMock.VerifyAll();
        _strategyFactoryMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WithEmptyRole_ShouldThrow()
    {
        var request = new UserCreateRequest
        {
            Name = "Juan",
            Surname = "Perez",
            Email = "juan@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094123456",
            Password = "Valid1Password!@",
            Role = string.Empty,
        };

        _userService.CreateUser(request);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CreateUser_DuplicateEmail_ShouldThrow()
    {
        var existingUser = CreateTestUser(email: "juan@test.com");
        _userRepositoryMock.Setup(r => r.GetUserByEmail("juan@test.com")).Returns(existingUser);
        SetupPhoneStrategy();

        var request = new UserCreateRequest
        {
            Name = "Juan",
            Surname = "Perez",
            Email = "juan@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094123456",
            Password = "Valid1Password!@",
        };

        _userService.CreateUser(request);

        _userRepositoryMock.VerifyAll();
        _strategyFactoryMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void UpdateUser_EmailAlreadyUsedByOtherUser_ShouldThrow()
    {
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingUser = CreateTestUser("Old", "Name", "old@test.com", Role.Preparador);
        var otherUser = CreateTestUser("Otro", "Usuario", "nuevo@test.com");

        _userRepositoryMock.Setup(r => r.GetById(userId)).Returns(existingUser);
        _userRepositoryMock.Setup(r => r.GetUserByEmail("nuevo@test.com")).Returns(otherUser);
        SetupPhoneStrategy("+598", "094999888");

        var request = new UserUpdateRequest
        {
            Name = "Nuevo",
            Surname = "Nombre",
            Email = "nuevo@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094999888",
            Role = "Administrativo",
        };

        _userService.UpdateUser(adminId, userId, request);

        _userRepositoryMock.VerifyAll();
        _strategyFactoryMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void UpdateUser_UserNotFound_ShouldThrowKeyNotFoundException()
    {
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userRepositoryMock.Setup(r => r.GetById(userId)).Returns((User?)null);

        var request = new UserUpdateRequest
        {
            Name = "Nuevo",
            Surname = "Nombre",
            Email = "nuevo@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094999888",
            Role = "Administrativo",
        };

        _userService.UpdateUser(adminId, userId, request);

        _userRepositoryMock.VerifyAll();
    }
}
