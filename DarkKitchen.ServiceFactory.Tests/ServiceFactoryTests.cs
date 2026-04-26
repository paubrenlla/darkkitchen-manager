using DarkKitchen.BusinessLogic;
using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.DataAccess;
using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.IDataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DarkKitchen.ServiceFactory.Tests;

[TestClass]
public class ServiceFactoryTests
{
    private IServiceCollection _services = null!;

    [TestInitialize]
    public void Setup()
    {
        _services = new ServiceCollection();

        var configMock = new Mock<IConfiguration>();
        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(s => s.Value).Returns("test_secret_key_that_is_long_enough_for_hmac");
        configMock.Setup(c => c.GetSection("JwtConfig:Secret")).Returns(configSectionMock.Object);
        _services.AddSingleton(configMock.Object);
        _services.AddSingleton<IProductRepository, InMemoryProductRepository>();
    }

    [TestMethod]
    public void AddProjectServices_RegistersIUserRepository()
    {
        _services.AddProjectServices();

        var provider = _services.BuildServiceProvider();
        var repository = provider.GetService<IUserRepository>();

        Assert.IsNotNull(repository);
        Assert.IsInstanceOfType(repository, typeof(InMemoryUserRepository));
    }

    [TestMethod]
    public void AddProjectServices_RegistersIAuthService()
    {
        _services.AddProjectServices();

        var provider = _services.BuildServiceProvider();
        var authService = provider.GetService<IAuthService>();

        Assert.IsNotNull(authService);
        Assert.IsInstanceOfType(authService, typeof(AuthService));
    }

    [TestMethod]
    public void AddProjectServices_RegistersITokenService()
    {
        _services.AddProjectServices();

        var provider = _services.BuildServiceProvider();
        var tokenService = provider.GetService<ITokenService>();

        Assert.IsNotNull(tokenService);
        Assert.IsInstanceOfType(tokenService, typeof(TokenService));
    }

    [TestMethod]
    public void AddProjectServices_RegistersIProductService()
    {
        _services.AddProjectServices();

        var provider = _services.BuildServiceProvider();
        var productService = provider.GetService<IProductService>();

        Assert.IsNotNull(productService);
        Assert.IsInstanceOfType(productService, typeof(ProductService));
    }

    [TestMethod]
    public void AddProjectServices_ReturnsServiceCollection()
    {
        var result = _services.AddProjectServices();

        Assert.AreSame(_services, result);
    }

    [TestMethod]
    public void AddProjectServices_RegistersIPasswordHasher()
    {
        _services.AddProjectServices();

        var provider = _services.BuildServiceProvider();
        var hasher = provider.GetService<IPasswordHasher>();

        Assert.IsNotNull(hasher);
        Assert.IsInstanceOfType(hasher, typeof(BCryptHasher));
    }
}
