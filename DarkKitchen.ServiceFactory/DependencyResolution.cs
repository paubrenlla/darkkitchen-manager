using DarkKitchen.BusinessLogic;
using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.BusinessLogic.PhoneNumber;
using DarkKitchen.DataAccess;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.IBusinessLogic.IPhoneNumber;
using DarkKitchen.IDataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.ServiceFactory;

public static class DependencyResolution
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddSingleton<DarkKitchen.Domain.Users.IPhoneValidationStrategy, DarkKitchen.Domain.Users.UruguayPhoneValidationStrategy>();
        services.AddScoped<IPhoneStrategyFactory, PhoneStrategyFactory>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
