using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DarkKitchen.Plugin.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DarkKitchen.Plugin.JsonImporter.Tests
{
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
            if (File.Exists(_testFilePath))
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
                new ProductImportDto
                {
                    Code = "P001",
                    Name = "Test Product",
                    Description = "Test Description",
                    LineName = "Line 1",
                    CategoryName = "Category 1",
                    Price = 99.99m,
                    ImagesBase64 = new List<string> { "base64image1", "base64image2" }
                }
            };

            var jsonContent = JsonSerializer.Serialize(products);
            File.WriteAllText(_testFilePath, jsonContent);

            var result = _importer.ImportProducts(_testFilePath).ToList();

            Assert.AreEqual(1, result.Count);
            var importedProduct = result.First();
            Assert.AreEqual("P001", importedProduct.Code);
            Assert.AreEqual("Test Product", importedProduct.Name);
            Assert.AreEqual("Test Description", importedProduct.Description);
            Assert.AreEqual("Line 1", importedProduct.LineName);
            Assert.AreEqual("Category 1", importedProduct.CategoryName);
            Assert.AreEqual(99.99m, importedProduct.Price);
            Assert.IsNotNull(importedProduct.ImagesBase64);
            Assert.AreEqual(2, importedProduct.ImagesBase64.Count);
            Assert.AreEqual("base64image1", importedProduct.ImagesBase64[0]);
            Assert.AreEqual("base64image2", importedProduct.ImagesBase64[1]);
        }
    }
}
