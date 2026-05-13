using DarkKitchen.Plugin.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.ServiceFactory.Tests;

[TestClass]
public class PluginLoaderExtensionsTests
{
    private IServiceCollection _services = null!;
    private string _tempFolder = null!;

    [TestInitialize]
    public void Setup()
    {
        _services = new ServiceCollection();
        _tempFolder = Path.Combine(Path.GetTempPath(), $"plugins_{Guid.NewGuid()}");
    }

    [TestCleanup]
    public void Cleanup()
    {
        if(Directory.Exists(_tempFolder))
        {
            Directory.Delete(_tempFolder, true);
        }
    }

    [TestMethod]
    public void AddProductImportersPlugins_WhenFolderDoesNotExist_ShouldCreateItAndReturnServices()
    {
        _services.AddProductImportersPlugins(_tempFolder);

        Assert.IsTrue(Directory.Exists(_tempFolder));
        Assert.AreEqual(0, _services.Count);
    }

    [TestMethod]
    public void AddProductImportersPlugins_WhenDirectoryCreationFails_ShouldReturnServices()
    {
        // Creamos un archivo con el mismo nombre que intentará usar como directorio
        // para forzar una excepción en Directory.CreateDirectory
        var filePath = Path.Combine(Path.GetTempPath(), $"plugins_file_{Guid.NewGuid()}");
        File.WriteAllText(filePath, "dummy content");

        try
        {
            _services.AddProductImportersPlugins(filePath);

            // Si no tira excepción, el catch funcionó
            Assert.AreEqual(0, _services.Count);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [TestMethod]
    public void AddProductImportersPlugins_WhenFolderExistsButEmpty_ShouldReturnServices()
    {
        Directory.CreateDirectory(_tempFolder);

        _services.AddProductImportersPlugins(_tempFolder);

        Assert.AreEqual(0, _services.Count);
    }

    [TestMethod]
    public void AddProductImportersPlugins_WithCorruptedDll_ShouldIgnoreAndReturnServices()
    {
        Directory.CreateDirectory(_tempFolder);
        var corruptDllPath = Path.Combine(_tempFolder, "corrupt.dll");
        File.WriteAllText(corruptDllPath, "This is not a real dll");

        _services.AddProductImportersPlugins(_tempFolder);

        Assert.AreEqual(0, _services.Count);
    }

    [TestMethod]
    public void AddProductImportersPlugins_WithValidDll_ShouldRegisterPlugins()
    {
        Directory.CreateDirectory(_tempFolder);

        // Usamos la DLL real de nuestro JsonImporter para la prueba
        var validDllPath = typeof(DarkKitchen.Plugin.JsonImporter.JsonProductImporter).Assembly.Location;
        File.Copy(validDllPath, Path.Combine(_tempFolder, "DarkKitchen.Plugin.JsonImporter.dll"));

        _services.AddProductImportersPlugins(_tempFolder);

        // Debería haber registrado 1 servicio (IProductImporter -> JsonProductImporter)
        Assert.AreEqual(1, _services.Count);

        var descriptor = _services.FirstOrDefault();
        Assert.IsNotNull(descriptor);
        Assert.AreEqual(typeof(IProductImporter), descriptor.ServiceType);
        Assert.AreEqual(typeof(DarkKitchen.Plugin.JsonImporter.JsonProductImporter), descriptor.ImplementationType);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }
}
