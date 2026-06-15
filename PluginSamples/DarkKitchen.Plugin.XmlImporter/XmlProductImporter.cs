using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DarkKitchen.Plugin.Contracts;

namespace DarkKitchen.Plugin.XmlImporter;

[XmlRoot("Products")]
public class XmlProductList
{
    [XmlElement("Product")]
    public List<XmlProductDto> Products { get; set; } = new();
}

public class XmlProductDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    [XmlArray("Images")]
    [XmlArrayItem("Image")]
    public List<XmlImageDto> Images { get; set; } = new();
}

public class XmlImageDto
{
    public string Url { get; set; } = string.Empty;
    public long Size { get; set; }
}

public class XmlProductImporter : IProductImporter
{
    public string Name => "XML Importer";

    public IEnumerable<ProductImportDto> ImportProducts(string filePath)
    {
        if(!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Archivo no encontrado: {filePath}");
        }

        var serializer = new XmlSerializer(typeof(XmlProductList));
        using var reader = new StreamReader(filePath);
        var xmlList = (XmlProductList)serializer.Deserialize(reader)!;

        return xmlList.Products.Select(p => new ProductImportDto
        {
            Code = p.Code,
            Name = p.Name,
            Description = p.Description,
            LineName = p.Line,
            CategoryName = p.Category,
            Price = p.Price,
            Images = p.Images.Select(img => new ImageImportDto
            {
                Url = img.Url,
                SizeInBytes = img.Size
            }).ToList()
        });
    }
}
