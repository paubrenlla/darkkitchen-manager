namespace DarkKitchen.Models.DTOs
{
    public class ProductImportResponse
    {
        public int TotalProcessed { get; set; }
        public int Successful { get; set; }
        public int Failed { get; set; }
        public List<ProductResponse> ImportedProducts { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
