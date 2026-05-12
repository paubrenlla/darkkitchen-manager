using System.Text.Json;
using DarkKitchen.Plugin.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DarkKitchen.Plugin.JsonImporter.Tests;

[TestClass]
public class JsonProductImporterTests
{
    private JsonProductImporter _importer = null!;
    private string _testFilePath = null!;

    [TestInitialize]
    public void Setup()
    {
        _importer = new JsonProductImporter();
        _testFilePath = Path.Combine(Path.GetTempPath(), $"test_products_{Guid.NewGuid()}.json");
    }

    [TestCleanup]
    public void Cleanup()
    {
        if(File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [TestMethod]
    public void Name_ShouldReturnJsonImporter()
    {
        Assert.AreEqual("JSON Importer", _importer.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void ImportProducts_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
    {
        _importer.ImportProducts("non_existent_file.json");
    }

    [TestMethod]
    public void ImportProducts_WithValidJson_ShouldReturnProducts()
    {
        var products = new List<ProductImportDto>
        {
            new()
            {
                Code = "P001",
                Name = "Test Product",
                Description = "Test Description",
                LineName = "Line 1",
                CategoryName = "Category 1",
                Price = 99.99m,
                ImagePaths = new List<string>
                {
                    "https://img.darkkitchen.com/limonada.jpg", "https://img.darkkitchen.com/limonada2.jpg"
                }
            }
        };

        var jsonContent = JsonSerializer.Serialize(products);
        File.WriteAllText(_testFilePath, jsonContent);

        var result = _importer.ImportProducts(_testFilePath).ToList();

        Assert.AreEqual(1, result.Count);
        ProductImportDto importedProduct = result.First();
        Assert.AreEqual("P001", importedProduct.Code);
        Assert.AreEqual("Test Product", importedProduct.Name);
        Assert.AreEqual("Test Description", importedProduct.Description);
        Assert.AreEqual("Line 1", importedProduct.LineName);
        Assert.AreEqual("Category 1", importedProduct.CategoryName);
        Assert.AreEqual(99.99m, importedProduct.Price);
        Assert.IsNotNull(importedProduct.ImagePaths);
        Assert.AreEqual(2, importedProduct.ImagePaths.Count);
        Assert.AreEqual("https://img.darkkitchen.com/limonada.jpg", importedProduct.ImagePaths[0]);
        Assert.AreEqual("https://img.darkkitchen.com/limonada2.jpg", importedProduct.ImagePaths[1]);
    }
}
