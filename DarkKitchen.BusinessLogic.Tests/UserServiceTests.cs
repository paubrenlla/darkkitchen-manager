using DarkKitchen.Domain.Users;
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
        _userRepositoryMock = new Mock<IUserRepository>();
        _strategyFactoryMock = new Mock<IPhoneStrategyFactory>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed_password");
        _userService = new UserService(
            _userRepositoryMock.Object,
            _strategyFactoryMock.Object,
            _passwordHasherMock.Object);
    }

    [TestMethod]
    public void CreateUser_ValidRequest_CreatesUserAndCallsRepository()
    {
        var request = new UserCreateRequest
        {
            Name = "Carlos",
            Surname = "Perez",
            Email = "carlos@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094123456",
            Password = "Valid1Password!@"
        };

        var mockStrategy = new Mock<IPhoneValidationStrategy>();
        mockStrategy.Setup(s => s.CountryPrefix).Returns("+598");
        mockStrategy.Setup(s => s.IsValid("094123456")).Returns(true);

        _strategyFactoryMock.Setup(f => f.GetStrategy("+598")).Returns(mockStrategy.Object);

        var result = _userService.CreateUser(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Carlos", result.Name);
        _userRepositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Once);
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

        _strategyFactoryMock.Setup(f => f.GetStrategy("+123"))
            .Throws(new NotSupportedException("Country prefix not supported."));

        _userService.CreateUser(request);
    }

    [TestMethod]
    public void CreateUser_WithExplicitRole_ShouldUseProvidedRole()
    {
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

        var mockStrategy = new Mock<IPhoneValidationStrategy>();
        mockStrategy.Setup(s => s.CountryPrefix).Returns("+598");
        mockStrategy.Setup(s => s.IsValid("094123456")).Returns(true);

        _strategyFactoryMock.Setup(f => f.GetStrategy("+598")).Returns(mockStrategy.Object);

        var result = _userService.CreateUser(request);

        Assert.IsNotNull(result);
        _userRepositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public void GetUsers_ShouldDelegateToRepository()
    {
        IPhoneValidationStrategy strategy = new UruguayPhoneValidationStrategy();
        var phone = Domain.Users.PhoneNumber.Create("+598", "094123456", strategy);
        List<User> users = [new User("Juan", "Perez", "juan@test.com", phone, "Valid1Password!@", Role.Cliente, _passwordHasherMock.Object)];

        _userRepositoryMock.Setup(r => r.GetByNameAndSurname("Juan", "Perez")).Returns(users);

        IEnumerable<UserCreateResponse> result = _userService.GetUsers("Juan", "Perez");

        Assert.AreEqual(1, result.Count());
        _userRepositoryMock.Verify(r => r.GetByNameAndSurname("Juan", "Perez"), Times.Once);
    }

    [TestMethod]
    public void UpdateUser_ValidRequest_ShouldUpdateAndReturn()
    {
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        IPhoneValidationStrategy strategy = new UruguayPhoneValidationStrategy();
        var phone = Domain.Users.PhoneNumber.Create("+598", "094123456", strategy);
        var existingUser = new User("Old", "Name", "old@test.com", phone, "Valid1Password!@", Role.Preparador, _passwordHasherMock.Object);

        _userRepositoryMock.Setup(r => r.GetById(userId)).Returns(existingUser);

        var mockStrategy = new Mock<IPhoneValidationStrategy>();
        mockStrategy.Setup(s => s.CountryPrefix).Returns("+598");
        mockStrategy.Setup(s => s.IsValid("094999888")).Returns(true);
        _strategyFactoryMock.Setup(f => f.GetStrategy("+598")).Returns(mockStrategy.Object);

        var request = new UserUpdateRequest
        {
            Name = "Nuevo",
            Surname = "Nombre",
            Email = "nuevo@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094999888",
            Role = "Administrativo",
        };

        UserCreateResponse result = _userService.UpdateUser(adminId, userId, request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Nuevo", result.Name);
        _userRepositoryMock.Verify(r => r.Update(userId, It.IsAny<User>()), Times.Once);
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

        _userService.DeleteUser(adminId, userId);

        _userRepositoryMock.Verify(r => r.Delete(userId), Times.Once);
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

        var mockStrategy = new Mock<IPhoneValidationStrategy>();
        mockStrategy.Setup(s => s.CountryPrefix).Returns("+598");
        mockStrategy.Setup(s => s.IsValid("094123456")).Returns(true);
        _strategyFactoryMock.Setup(f => f.GetStrategy("+598")).Returns(mockStrategy.Object);

        UserCreateResponse result = _userService.CreateUser(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Administrativo", result.Role);
    }

    [TestMethod]
    public void CreateUser_WithPreparadorRole_ShouldSucceed()
    {
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

        var mockStrategy = new Mock<IPhoneValidationStrategy>();
        mockStrategy.Setup(s => s.CountryPrefix).Returns("+598");
        mockStrategy.Setup(s => s.IsValid("094123456")).Returns(true);
        _strategyFactoryMock.Setup(f => f.GetStrategy("+598")).Returns(mockStrategy.Object);

        UserCreateResponse result = _userService.CreateUser(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Preparador", result.Role);
    }

    [TestMethod]
    public void CreateUser_WithNullRole_ShouldCreateCliente()
    {
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

        var mockStrategy = new Mock<IPhoneValidationStrategy>();
        mockStrategy.Setup(s => s.CountryPrefix).Returns("+598");
        mockStrategy.Setup(s => s.IsValid("094123456")).Returns(true);
        _strategyFactoryMock.Setup(f => f.GetStrategy("+598")).Returns(mockStrategy.Object);

        UserCreateResponse result = _userService.CreateUser(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Cliente", result.Role);
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
        var request = new UserCreateRequest
        {
            Name = "Juan",
            Surname = "Perez",
            Email = "juan@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094123456",
            Password = "Valid1Password!@",
        };

        var mockStrategy = new Mock<IPhoneValidationStrategy>();
        mockStrategy.Setup(s => s.CountryPrefix).Returns("+598");
        mockStrategy.Setup(s => s.IsValid("094123456")).Returns(true);
        _strategyFactoryMock.Setup(f => f.GetStrategy("+598")).Returns(mockStrategy.Object);

        var existingPhone = Domain.Users.PhoneNumber.Create("+598", "094123456", mockStrategy.Object);
        var existingUser = new User("Otro", "Usuario", "juan@test.com", existingPhone, "Valid1Password!@", Role.Cliente, _passwordHasherMock.Object);
        _userRepositoryMock.Setup(r => r.GetUserByEmail("juan@test.com")).Returns(existingUser);

        _userService.CreateUser(request);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void UpdateUser_EmailAlreadyUsedByOtherUser_ShouldThrow()
    {
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        IPhoneValidationStrategy strategy = new UruguayPhoneValidationStrategy();
        var phone = Domain.Users.PhoneNumber.Create("+598", "094123456", strategy);
        var existingUser = new User("Old", "Name", "old@test.com", phone, "Valid1Password!@", Role.Preparador, _passwordHasherMock.Object);
        var otherUser = new User("Otro", "Usuario", "nuevo@test.com", phone, "Valid1Password!@", Role.Cliente, _passwordHasherMock.Object);

        _userRepositoryMock.Setup(r => r.GetById(userId)).Returns(existingUser);
        _userRepositoryMock.Setup(r => r.GetUserByEmail("nuevo@test.com")).Returns(otherUser);

        var mockStrategy = new Mock<IPhoneValidationStrategy>();
        mockStrategy.Setup(s => s.CountryPrefix).Returns("+598");
        mockStrategy.Setup(s => s.IsValid("094999888")).Returns(true);
        _strategyFactoryMock.Setup(f => f.GetStrategy("+598")).Returns(mockStrategy.Object);

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
    }
}
