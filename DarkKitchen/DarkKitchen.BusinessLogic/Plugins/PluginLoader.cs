using System.Reflection;
using DarkKitchen.Plugin.Contracts;

namespace DarkKitchen.BusinessLogic.Plugins;

public static class PluginLoader
{
    public static IEnumerable<IProductImporter> LoadFromPath(string pluginsPath)
    {
        var importers = new List<IProductImporter>();

        if(!Directory.Exists(pluginsPath))
        {
            return importers;
        }

        var importerType = typeof(IProductImporter);

        foreach(var file in Directory.GetFiles(pluginsPath, "*.dll"))
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                var types = assembly.GetTypes()
                    .Where(t => importerType.IsAssignableFrom(t)
                                && !t.IsInterface && !t.IsAbstract);

                foreach(var type in types)
                {
                    importers.Add((IProductImporter)Activator.CreateInstance(type)!);
                }
            }
            catch
            {
                continue;
            }
        }

        return importers;
    }
}
