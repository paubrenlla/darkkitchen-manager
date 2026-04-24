namespace DarkKitchen.Domain;

public class ProductImage
{
    private const string RequiredExtension = ".jpg";
    public string FileName { get; private set; }
    public long SizeInBytes { get; private set; }

    public ProductImage(string fileName, long sizeInBytes)
    {
        ValidateFileName(fileName);
        FileName = fileName;
        SizeInBytes = sizeInBytes;
    }

    private static void ValidateFileName(string fileName)
    {
        if(string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("Image file name is required.");
        }

        if(!fileName.EndsWith(RequiredExtension, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Image must be in jpg format.");
        }
    }
}
