namespace DarkKitchen.Plugin.Contracts;

public interface IProductImporter
{
    string Name { get; }
    IEnumerable<ProductImportDto> ImportProducts(string filePath);
}
