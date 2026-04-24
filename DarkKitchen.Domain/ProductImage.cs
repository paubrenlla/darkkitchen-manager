namespace DarkKitchen.Domain;

public class ProductImage
{
    public string FileName { get; private set; }
    public long SizeInBytes { get; private set; }

    public ProductImage(string fileName, long sizeInBytes)
    {
        FileName = fileName;
        SizeInBytes = sizeInBytes;
    }
}
