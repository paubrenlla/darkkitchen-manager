using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DarkKitchen.Plugin.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.ServiceFactory;

public static class PluginLoaderExtensions
{
    public static IServiceCollection AddProductImportersPlugins(this IServiceCollection services, string pluginsPath)
    {
        if (!Directory.Exists(pluginsPath))
        {
            try
            {
                Directory.CreateDirectory(pluginsPath);
            }
            catch
            {
                // Si no se puede crear, simplemente no cargamos plugins
                return services;
            }
        }

        var dllFiles = Directory.GetFiles(pluginsPath, "*.dll");

        var importerType = typeof(IProductImporter);

        foreach (var file in dllFiles)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);

                var types = assembly.GetTypes()
                    .Where(t => importerType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var type in types)
                {
                    // Registramos cada implementación encontrada bajo la misma interfaz
                    services.AddScoped(importerType, type);
                }
            }
            catch (Exception)
            {
                // Ignoramos librerías que no son .NET válidas, corruptas o que fallan al cargar
                continue;
            }
        }

        return services;
    }
}
