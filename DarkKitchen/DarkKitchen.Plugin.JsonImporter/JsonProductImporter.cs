using System.Text.Json;
using DarkKitchen.Plugin.Contracts;

namespace DarkKitchen.Plugin.JsonImporter;

public class JsonProductImporter : IProductImporter
{
    public string Name => "JSON Importer";

    public IEnumerable<ProductImportDto> ImportProducts(string filePath)
    {
        if(!File.Exists(filePath))
        {
            throw new FileNotFoundException($"El archivo no fue encontrado: {filePath}");
        }

        var jsonContent = File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var products = JsonSerializer.Deserialize<List<ProductImportDto>>(jsonContent, options);

        return products ?? [];
    }
}
