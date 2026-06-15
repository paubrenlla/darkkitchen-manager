namespace DarkKitchen.IBusinessLogic;

public interface IPluginService
{
    IEnumerable<string> GetAvailableImporters();
}
