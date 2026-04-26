using DarkKitchen.BusinessLogic;
using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.BusinessLogic.PhoneNumber;
using DarkKitchen.DataAccess;
using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.IBusinessLogic.IPhoneNumber;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.ServiceFactory;

public static class DependencyResolution
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DarkKitchenContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DarkKitchenDB")));

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IProductRepository, InMemoryProductRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddSingleton<IPasswordHasher, BCryptHasher>();
        services.AddSingleton<IPhoneValidationStrategy, UruguayPhoneValidationStrategy>();
        services.AddScoped<IPhoneStrategyFactory, PhoneStrategyFactory>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IPromotionRepository, InMemoryPromotionRepository>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IShippingStrategy>(sp => new ExpressShippingStrategy(150m));
        services.AddScoped<IShippingStrategy>(sp => new TwentyFourHoursShippingStrategy(50m));
        services.AddScoped<IShippingCostCalculator, ShippingCostCalculator>();

        return services;
    }
}
