using System.Collections.Generic;
using DarkKitchen.Plugin.Contracts;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DarkKitchen.WebApi.Tests
{
    [TestClass]
    public class PluginsControllerTests
    {
        [TestMethod]
        public void GetImporters_ShouldReturnOkWithImporterNames()
        {
            var mockImporter1 = new Mock<IProductImporter>();
            mockImporter1.Setup(i => i.Name).Returns("Importer 1");

            var mockImporter2 = new Mock<IProductImporter>();
            mockImporter2.Setup(i => i.Name).Returns("Importer 2");

            var importers = new List<IProductImporter> { mockImporter1.Object, mockImporter2.Object };

            var controller = new PluginsController(importers);

            var result = controller.GetImporters() as OkObjectResult;

            Assert.IsNotNull(result);
            var resultValue = result.Value as List<string>;
            Assert.IsNotNull(resultValue);
            Assert.AreEqual(2, resultValue.Count);
            Assert.AreEqual("Importer 1", resultValue[0]);
            Assert.AreEqual("Importer 2", resultValue[1]);
        }
    }
}
