namespace DarkKitchen.Domain;

public class ProductImage
{
    private const int MaxSizeInBytes = 500 * 1024;
    private const string RequiredExtension = ".jpg";
    public string FileName { get; private set; }
    public long SizeInBytes { get; private set; }

    public ProductImage(string fileName, long sizeInBytes)
    {
        ValidateFileName(fileName);
        ValidateSize(sizeInBytes);
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

    private static void ValidateSize(long sizeInBytes)
    {
        if(sizeInBytes <= 0)
        {
            throw new ArgumentException("Image size must be greater than zero.");
        }

        if(sizeInBytes > MaxSizeInBytes)
        {
            throw new ArgumentException("Image size must not exceed 500KB.");
        }
    }
}
