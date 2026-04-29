namespace DarkKitchen.Domain.Products;

public class ProductImage
{
    private const string RequiredExtension = ".jpg";
    private const long MaxSizeInBytes = 500 * 1024;

    public string Url { get; private set; }
    public long SizeInBytes { get; private set; }
    public Guid Id { get; private set; }

    public ProductImage(string url, long sizeInBytes)
    {
        ValidateUrl(url);
        ValidateSize(sizeInBytes);

        Url = url;
        SizeInBytes = sizeInBytes;
        Id = Guid.NewGuid();
    }

    private static void ValidateUrl(string url)
    {
        if(string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Image URL is required.");
        }

        if(!url.EndsWith(RequiredExtension, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Image URL must point to a jpg file.");
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
