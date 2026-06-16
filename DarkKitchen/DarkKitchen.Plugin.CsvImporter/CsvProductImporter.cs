using System.Collections.Generic;
using System.IO;
using System.Linq;
using DarkKitchen.Plugin.Contracts;

namespace DarkKitchen.Plugin.CsvImporter;

public class CsvProductImporter : IProductImporter
{
    public string Name => "CSV Importer";

    public IEnumerable<ProductImportDto> ImportProducts(string filePath)
    {
        if(!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Archivo no encontrado: {filePath}");
        }

        var products = new List<ProductImportDto>();
        var lines = File.ReadAllLines(filePath);

        // Omitimos el header si existe (asumimos que la primera línea es el header)
        foreach(var line in lines.Skip(1))
        {
            if(string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split(',');

            if(parts.Length < 6)
            {
                continue;
            }

            var dto = new ProductImportDto
            {
                Code = parts[0].Trim(),
                Name = parts[1].Trim(),
                Description = parts[2].Trim(),
                LineName = parts[3].Trim(),
                CategoryName = parts[4].Trim(),
                Price = decimal.TryParse(parts[5].Trim(), System.Globalization.CultureInfo.InvariantCulture,
                    out var p)
                    ? p
                    : 0,
                Images = []
            };

            // Parsear imágenes (formato: url1;url2, size1;size2)
            if(parts.Length >= 8)
            {
                var urls = parts[6].Split(';');
                var sizes = parts[7].Split(';');

                for(var i = 0; i < urls.Length; i++)
                {
                    if(!string.IsNullOrWhiteSpace(urls[i]))
                    {
                        var size = i < sizes.Length && long.TryParse(sizes[i], out var s) ? s : 0;
                        dto.Images.Add(new ImageImportDto { Url = urls[i].Trim(), SizeInBytes = size });
                    }
                }
            }

            products.Add(dto);
        }

        return products;
    }
}
