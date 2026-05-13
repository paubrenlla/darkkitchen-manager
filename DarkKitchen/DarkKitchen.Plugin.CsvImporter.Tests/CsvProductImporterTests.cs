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

    [TestMethod]
    public void ImportProducts_WithInvalidDataTypes_ShouldUseDefaultValues()
    {
        var csvLines = new[]
        {
            "Code,Name,Description,Line,Category,Price,ImageUrls,ImageSizes",
            "INV01,Name,Desc,L,C,INVALID_PRICE,https://img.com/1.jpg,INVALID_SIZE"
        };
        File.WriteAllLines(_testFilePath, csvLines);

        var result = _importer.ImportProducts(_testFilePath).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(0, result.First().Price);
        Assert.AreEqual(0, result.First().Images!.First().SizeInBytes);
    }

    [TestMethod]
    public void ImportProducts_WithMismatchedImagesAndSizes_ShouldHandleIt()
    {
        var csvLines = new[]
        {
            "Code,Name,Description,Line,Category,Price,ImageUrls,ImageSizes",
            "MIS01,Name,Desc,L,C,100,url1;url2;url3,100;200", // 3 URLs, solo 2 tamaños
            "EMPTY_IMG,Name,Desc,L,C,100,url1;;url3,100;200;300" // URL vacía en el medio
        };
        File.WriteAllLines(_testFilePath, csvLines);

        var result = _importer.ImportProducts(_testFilePath).ToList();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(3, result[0].Images!.Count);
        Assert.AreEqual(0, result[0].Images!.ElementAt(2).SizeInBytes); // El tercer tamaño no existe, usa 0
        Assert.AreEqual(2, result[1].Images!.Count); // Ignora la URL vacía del medio
    }
}
