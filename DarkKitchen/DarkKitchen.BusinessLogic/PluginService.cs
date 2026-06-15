using DarkKitchen.BusinessLogic.Plugins;
using DarkKitchen.IBusinessLogic;

public class PluginService(string pluginsPath) : IPluginService
{
    private readonly string _pluginsPath = pluginsPath;

    public PluginService()
        : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"))
    {
    }

    public IEnumerable<string> GetAvailableImporters()
    {
        return PluginLoader
            .LoadFromPath(_pluginsPath)
            .Select(i => i.Name);
    }
}
