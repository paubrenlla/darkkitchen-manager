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
                Images =
                [
                    new ImageImportDto { Url = "https://img.darkkitchen.com/limonada.jpg", SizeInBytes = 10240 },
                    new ImageImportDto { Url = "https://img.darkkitchen.com/limonada2.jpg", SizeInBytes = 20480 }
                ]
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
        Assert.IsNotNull(importedProduct.Images);
        Assert.AreEqual(2, importedProduct.Images.Count);
        Assert.AreEqual("https://img.darkkitchen.com/limonada.jpg", importedProduct.Images[0].Url);
        Assert.AreEqual(10240, importedProduct.Images[0].SizeInBytes);
        Assert.AreEqual("https://img.darkkitchen.com/limonada2.jpg", importedProduct.Images[1].Url);
        Assert.AreEqual(20480, importedProduct.Images[1].SizeInBytes);
    }

    [TestMethod]
    public void ImportProducts_WithNullJson_ShouldReturnEmptyList()
    {
        File.WriteAllText(_testFilePath, "null");

        var result = _importer.ImportProducts(_testFilePath);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }
}
