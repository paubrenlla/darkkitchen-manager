using DarkKitchen.BusinessLogic;
using DarkKitchen.Plugin.JsonImporter;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class PluginServiceTests
{
    private string _tempFolder = null!;

    [TestInitialize]
    public void Setup()
    {
        _tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempFolder);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if(Directory.Exists(_tempFolder))
        {
            Directory.Delete(_tempFolder, recursive: true);
        }
    }

    [TestMethod]
    public void GetAvailableImporters_EmptyFolder_ShouldReturnEmpty()
    {
        var service = new PluginService(_tempFolder);

        var result = service.GetAvailableImporters().ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetAvailableImporters_WithValidDll_ShouldReturnImporterName()
    {
        var dllPath = typeof(JsonProductImporter).Assembly.Location;
        File.Copy(dllPath, Path.Combine(_tempFolder, "DarkKitchen.Plugin.JsonImporter.dll"));

        var service = new PluginService(_tempFolder);

        var result = service.GetAvailableImporters().ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("JSON Importer", result[0]);
    }

    [TestMethod]
    public void GetAvailableImporters_NonExistentFolder_ShouldReturnEmpty()
    {
        var service = new PluginService(Path.Combine(Path.GetTempPath(), "carpeta-que-no-existe"));

        var result = service.GetAvailableImporters().ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetAvailableImporters_AfterRemovingDll_ShouldNotReturnImporter()
    {
        var dllPath = typeof(JsonProductImporter).Assembly.Location;
        var destination = Path.Combine(_tempFolder, "DarkKitchen.Plugin.JsonImporter.dll");
        File.Copy(dllPath, destination);

        var service = new PluginService(_tempFolder);
        Assert.AreEqual(1, service.GetAvailableImporters().Count());

        File.Delete(destination);

        var result = service.GetAvailableImporters().ToList();
        Assert.AreEqual(0, result.Count);
    }
}
