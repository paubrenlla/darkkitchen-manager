using System.Diagnostics.CodeAnalysis;
using DarkKitchen.BusinessLogic;
using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.BusinessLogic.Events;
using DarkKitchen.BusinessLogic.Handlers;
using DarkKitchen.BusinessLogic.PhoneNumber;
using DarkKitchen.DataAccess;
using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Orders.Delivery;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;
using DarkKitchen.Domain.Users.Encryptor;
using DarkKitchen.Domain.Users.PhoneValidations;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.IBusinessLogic.IPhoneNumber;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.ServiceFactory;
[ExcludeFromCodeCoverage]
public static class DependencyResolution
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DarkKitchenContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DarkKitchenDB")));

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderEnricher, OrderEnricher>();
        services.AddScoped<IOrderRepository, SqlOrderRepository>();
        services.AddScoped<IUserRepository, SqlUserRepository>();
        services.AddScoped<IProductRepository, SqlProductRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddSingleton<IPasswordHasher, BCryptHasher>();
        services.AddSingleton<IPhoneValidationStrategy, UruguayPhoneValidationStrategy>();
        services.AddScoped<IPhoneStrategyFactory, PhoneStrategyFactory>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPromotionRepository, SqlPromotionRepository>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IShippingCostCalculator, ShippingCostCalculator>();
        services.AddScoped<IShippingTypeRepository, SqlShippingTypeRepository>();
        services.AddScoped<IShippingTypeService, ShippingTypeService>();

        services.AddScoped<IAuditRepository, SqlAuditRepository>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

        services.AddScoped<ProductAuditHandler>();
        services.AddScoped<IAuditEventHandler<EntityCreatedEvent<Product>>, ProductAuditHandler>();
        services.AddScoped<IAuditEventHandler<EntityModifiedEvent<Product>>, ProductAuditHandler>();
        services.AddScoped<IAuditEventHandler<EntityDeactivatedEvent<Product>>, ProductAuditHandler>();
        services.AddScoped<IAuditEventHandler<EntityActivatedEvent<Product>>, ProductAuditHandler>();

        services.AddScoped<PromotionAuditHandler>();
        services.AddScoped<IAuditEventHandler<EntityCreatedEvent<Promotion>>, PromotionAuditHandler>();
        services.AddScoped<IAuditEventHandler<EntityModifiedEvent<Promotion>>, PromotionAuditHandler>();

        services.AddScoped<IPluginService, PluginService>();
        var pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
        services.AddProductImportersPlugins(pluginsPath);

        return services;
    }

    public static void ApplyMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DarkKitchenContext>();
        context.Database.Migrate();
    }
}
