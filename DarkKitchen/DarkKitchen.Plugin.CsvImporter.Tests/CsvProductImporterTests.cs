using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DarkKitchen.Plugin.CsvImporter.Tests;

[TestClass]
public class CsvProductImporterTests
{
    private CsvProductImporter _importer = null!;
    private string _testFilePath = null!;

    [TestInitialize]
    public void Setup()
    {
        _importer = new CsvProductImporter();
        _testFilePath = Path.Combine(Path.GetTempPath(), $"test_products_{Guid.NewGuid()}.csv");
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
    public void Name_ShouldReturnCsvImporter()
    {
        Assert.AreEqual("CSV Importer", _importer.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void ImportProducts_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
    {
        _importer.ImportProducts("non_existent_file.csv");
    }

    [TestMethod]
    public void ImportProducts_WithValidCsv_ShouldReturnProducts()
    {
        var csvLines = new[]
        {
            "Code,Name,Description,Line,Category,Price,ImageUrls,ImageSizes",
            "CSV01,Test Product,Test Description,Line 1,Category 1,99.99,https://img.com/1.jpg;https://img.com/2.jpg,10240;20480"
        };
        File.WriteAllLines(_testFilePath, csvLines);

        var result = _importer.ImportProducts(_testFilePath).ToList();

        Assert.AreEqual(1, result.Count);
        var importedProduct = result.First();
        Assert.AreEqual("CSV01", importedProduct.Code);
        Assert.AreEqual("Test Product", importedProduct.Name);
        Assert.AreEqual(99.99m, importedProduct.Price);
        Assert.AreEqual(2, importedProduct.Images.Count);
        Assert.AreEqual("https://img.com/1.jpg", importedProduct.Images[0].Url);
        Assert.AreEqual(10240, importedProduct.Images[0].SizeInBytes);
    }

    [TestMethod]
    public void ImportProducts_WithMalformedLine_ShouldSkipIt()
    {
        var csvLines = new[]
        {
            "Code,Name,Description,Line,Category,Price,ImageUrls,ImageSizes",
            "INCOMPLETE,OnlyThree,Parts" // Debería skipear esta línea
        };
        File.WriteAllLines(_testFilePath, csvLines);

        var result = _importer.ImportProducts(_testFilePath).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void ImportProducts_WithEmptyLines_ShouldIgnoreThem()
    {
        var csvLines = new[]
        {
            "Code,Name,Description,Line,Category,Price,ImageUrls,ImageSizes",
            string.Empty,
            "   ",
            "CSV01,Valid,Valid Desc,L,C,100,https://img.com/1.jpg,100",
            string.Empty
        };
        File.WriteAllLines(_testFilePath, csvLines);

        var result = _importer.ImportProducts(_testFilePath).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("CSV01", result.First().Code);
    }
}
