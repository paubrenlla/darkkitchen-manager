using DarkKitchen.BusinessLogic.Plugins;
using DarkKitchen.IBusinessLogic;

public class PluginService : IPluginService
{
    private readonly string _pluginsPath;

    public PluginService()
        : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins")) { }

    public PluginService(string pluginsPath)
    {
        _pluginsPath = pluginsPath;
    }

    public IEnumerable<string> GetAvailableImporters()
    {
        return PluginLoader
            .LoadFromPath(_pluginsPath)
            .Select(i => i.Name);
    }
}
