using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class PromotionService(
    IPromotionRepository promotionRepository,
    IProductRepository productRepository) : IPromotionService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IPromotionRepository _promotionRepository = promotionRepository;

    public IEnumerable<PromotionCreateResponse> GetPromotions(DateTime? date, string? line, string? productCode)
    {
        IEnumerable<Promotion>? promotions = _promotionRepository.GetAll();

        promotions = FilterByDate(promotions, date);
        promotions = FilterByLine(promotions, line);
        promotions = FilterByProductCode(promotions, productCode);

        return promotions.Select(Converter.ToPromotionCreateResponse);
    }

    public PromotionCreateResponse CreatePromotion(PromotionCreateRequest request)
    {
        IEnumerable<Product> allProducts = _productRepository.GetAll();
        var selectedProducts = allProducts
            .Where(p => request.ProductCodes.Contains(p.Code))
            .ToList();

        if(selectedProducts.Count != request.ProductCodes.Count)
        {
            throw new ArgumentException("Uno o más códigos de producto no son válidos.");
        }

        var promotion = new Promotion(
            request.Name,
            request.DiscountPercentage,
            request.StartDate,
            request.EndDate,
            selectedProducts);

        _promotionRepository.Add(promotion);
        return Converter.ToPromotionCreateResponse(promotion);
    }

    public PromotionCreateResponse UpdatePromotion(Guid id, PromotionCreateRequest request)
    {
        Promotion existingPromo = _promotionRepository.GetAll().FirstOrDefault(p => p.Id == id)
                                  ?? throw new KeyNotFoundException("La promoción no existe.");
        var selectedProducts = _productRepository.GetAll()
            .Where(p => request.ProductCodes.Contains(p.Code))
            .ToList();

        existingPromo.Update(
            request.Name,
            request.DiscountPercentage,
            request.StartDate,
            request.EndDate,
            selectedProducts);

        return Converter.ToPromotionCreateResponse(existingPromo);
    }

    public decimal GetBestDiscountForProduct(Guid productId, DateTime date)
    {
        throw new NotImplementedException();
    }

    private IEnumerable<Promotion> FilterByDate(IEnumerable<Promotion> promotions, DateTime? date)
    {
        if(!date.HasValue)
        {
            return promotions;
        }

        return promotions.Where(p =>
            date.Value >= p.StartDate && date.Value <= p.EndDate);
    }

    private IEnumerable<Promotion> FilterByLine(IEnumerable<Promotion> promotions, string? line)
    {
        if(string.IsNullOrWhiteSpace(line))
        {
            return promotions;
        }

        return promotions.Where(p =>
            p.Products.Any(prod =>
                prod.Line.Name.Equals(line, StringComparison.OrdinalIgnoreCase)));
    }

    private IEnumerable<Promotion> FilterByProductCode(IEnumerable<Promotion> promotions, string? productCode)
    {
        if(string.IsNullOrWhiteSpace(productCode))
        {
            return promotions;
        }

        return promotions.Where(p =>
            p.Products.Any(prod =>
                prod.Code.Equals(productCode, StringComparison.OrdinalIgnoreCase)));
    }
}
