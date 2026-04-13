using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.DataAccess;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.IDataAccess;
using DarkKitchen.ServiceFactory;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.ServiceFactory.Tests;

[TestClass]
public class ServiceFactoryTests
{
    private IServiceCollection _services = null!;

    [TestInitialize]
    public void Setup()
    {
        _services = new ServiceCollection();
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
        // TokenService requires IConfiguration, so we need to register it
        var configMock = new Moq.Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        _services.AddSingleton(configMock.Object);

        _services.AddProjectServices();

        var provider = _services.BuildServiceProvider();
        var tokenService = provider.GetService<ITokenService>();

        Assert.IsNotNull(tokenService);
        Assert.IsInstanceOfType(tokenService, typeof(TokenService));
    }

    [TestMethod]
    public void AddProjectServices_ReturnsServiceCollection()
    {
        var result = _services.AddProjectServices();

        Assert.AreSame(_services, result);
    }
}
