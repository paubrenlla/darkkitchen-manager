using DarkKitchen.BusinessLogic;
using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.DataAccess;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.IDataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.ServiceFactory;

public static class DependencyResolution
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
