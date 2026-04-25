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
        IEnumerable<Promotion> promotions = _promotionRepository.GetAll();

        if(date.HasValue)
        {
            promotions = promotions.Where(p => date.Value >= p.StartDate && date.Value <= p.EndDate);
        }

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
}
