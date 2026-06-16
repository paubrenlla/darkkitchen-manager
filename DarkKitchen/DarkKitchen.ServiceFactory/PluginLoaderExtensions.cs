using DarkKitchen.BusinessLogic.Plugins;
using DarkKitchen.Plugin.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.ServiceFactory;

public static class PluginLoaderExtensions
{
    public static IServiceCollection AddProductImportersPlugins(this IServiceCollection services, string pluginsPath)
    {
        if(!Directory.Exists(pluginsPath))
        {
            try
            {
                Directory.CreateDirectory(pluginsPath);
            }
            catch
            {
                return services;
            }
        }

        foreach(var importer in PluginLoader.LoadFromPath(pluginsPath))
        {
            services.AddScoped(typeof(IProductImporter), _ => importer);
        }

        return services;
    }
}
