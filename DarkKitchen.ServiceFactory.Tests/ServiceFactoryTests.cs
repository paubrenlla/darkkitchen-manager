using DarkKitchen.BusinessLogic;
using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.DataAccess;
using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.IDataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.ServiceFactory.Tests;

[TestClass]
public class ServiceFactoryTests
{
    private IServiceCollection _services = null!;
    private ServiceProvider _provider = null!;

    [TestInitialize]
    public void Setup()
    {
        _services = new ServiceCollection();

        var configValues = new Dictionary<string, string?>
        {
            { "JwtConfig:Secret", "test_secret_key_that_is_long_enough_for_hmac" },
            { "ConnectionStrings:DarkKitchenDB", "Server=localhost;Database=TestDB;Trusted_Connection=true;" },
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        _services.AddSingleton(configuration);
        _services.AddProjectServices(configuration);
        _provider = _services.BuildServiceProvider();
    }

    [TestMethod]
    public void AddProjectServices_RegistersIUserRepository()
    {
        var repository = _provider.GetService<IUserRepository>();

        Assert.IsNotNull(repository);
        Assert.IsInstanceOfType(repository, typeof(InMemoryUserRepository));
    }

    [TestMethod]
    public void AddProjectServices_RegistersIAuthService()
    {
        var authService = _provider.GetService<IAuthService>();

        Assert.IsNotNull(authService);
        Assert.IsInstanceOfType(authService, typeof(AuthService));
    }

    [TestMethod]
    public void AddProjectServices_RegistersITokenService()
    {
        var tokenService = _provider.GetService<ITokenService>();

        Assert.IsNotNull(tokenService);
        Assert.IsInstanceOfType(tokenService, typeof(TokenService));
    }

    [TestMethod]
    public void AddProjectServices_RegistersIProductService()
    {
        var productService = _provider.GetService<IProductService>();

        Assert.IsNotNull(productService);
        Assert.IsInstanceOfType(productService, typeof(ProductService));
    }

    [TestMethod]
    public void AddProjectServices_ReturnsServiceCollection()
    {
        Assert.IsNotNull(_provider);
    }

    [TestMethod]
    public void AddProjectServices_RegistersIPasswordHasher()
    {
        var hasher = _provider.GetService<IPasswordHasher>();

        Assert.IsNotNull(hasher);
        Assert.IsInstanceOfType(hasher, typeof(BCryptHasher));
    }
}
