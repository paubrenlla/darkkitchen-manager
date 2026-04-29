using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;
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

        DateTime dateToFilter = date ?? DateTime.Now;

        IEnumerable<Promotion> filtered = promotions.Where(p => p.IsVigente(dateToFilter));

        if(!string.IsNullOrWhiteSpace(line))
        {
            filtered = filtered.Where(p => p.Products.Any(prod =>
                prod.Line.Name.Equals(line, StringComparison.OrdinalIgnoreCase)));
        }

        if(!string.IsNullOrWhiteSpace(productCode))
        {
            filtered = filtered.Where(p => p.Products.Any(prod =>
                prod.Code.Equals(productCode, StringComparison.OrdinalIgnoreCase)));
        }

        return filtered.Select(Converter.ToPromotionCreateResponse);
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
        Promotion existingPromo = _promotionRepository.GetById(id)
                                  ?? throw new KeyNotFoundException("La promoción no existe.");

        var selectedProducts = _productRepository.GetAll()
            .Where(p => request.ProductCodes.Contains(p.Code))
            .ToList();

        if(selectedProducts.Count != request.ProductCodes.Count)
        {
            throw new ArgumentException("Uno o más códigos de producto no son válidos.");
        }

        existingPromo.Update(
            request.Name,
            request.DiscountPercentage,
            request.StartDate,
            request.EndDate,
            selectedProducts);

        _promotionRepository.Update(existingPromo);
        return Converter.ToPromotionCreateResponse(existingPromo);
    }

    public (string? PromotionName, decimal Discount) GetBestPromotionForProduct(Guid productId, DateTime date)
    {
        var activePromos = _promotionRepository.GetAll()
            .Where(p => p.IsVigente(date) && p.Products.Any(prod => prod.Id == productId))
            .ToList();

        if(!activePromos.Any())
        {
            return (null, 0m);
        }

        Promotion bestPromo = activePromos.OrderByDescending(p => p.DiscountPercentage).First();

        return (bestPromo.Name, bestPromo.DiscountPercentage);
    }
}
