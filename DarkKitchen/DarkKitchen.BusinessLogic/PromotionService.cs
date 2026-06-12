using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class PromotionService(
    IPromotionRepository promotionRepository,
    IProductRepository productRepository,
    IDomainEventPublisher publisher) : IPromotionService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IPromotionRepository _promotionRepository = promotionRepository;
    private readonly IDomainEventPublisher _publisher = publisher;

    public IEnumerable<Promotion> GetPromotions(DateTime? date, string? line, string? productCode)
    {
        if(date.HasValue && date.Value.TimeOfDay == TimeSpan.Zero)
        {
            date = date.Value.Date.AddDays(1).AddSeconds(-1);
        }

        IEnumerable<Promotion> filtered = _promotionRepository.GetAll();

        if(date.HasValue)
        {
            filtered = filtered.Where(p => p.IsVigente(date.Value));
        }

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

        return filtered;
    }

    public Promotion CreatePromotion(PromotionCreateRequest request, string responsibleUser)
    {
        var selectedProducts = GetValidatedProducts(request.ProductCodes);
        var promotion = new Promotion(request.Name, request.DiscountPercentage, request.StartDate, request.EndDate, selectedProducts);

        _promotionRepository.Add(promotion);
        _publisher.Publish(new EntityCreatedEvent<Promotion>
        {
            EntityId = promotion.Id,
            EntityName = "Promotion",
            ResponsibleUser = responsibleUser,
            NewState = promotion
        });

        return promotion;
    }

    public Promotion UpdatePromotion(Guid id, PromotionCreateRequest request, string responsibleUser)
    {
        Promotion existingPromo = _promotionRepository.GetById(id)
                                  ?? throw new KeyNotFoundException("La promoción no existe.");

        var selectedProducts = GetValidatedProducts(request.ProductCodes);
        var oldState = existingPromo.Clone();

        existingPromo.Update(request.Name, request.DiscountPercentage, request.StartDate, request.EndDate, selectedProducts);
        _promotionRepository.Update(existingPromo);

        _publisher.Publish(new EntityModifiedEvent<Promotion>
        {
            EntityId = existingPromo.Id,
            EntityName = "Promotion",
            ResponsibleUser = responsibleUser,
            OldState = oldState,
            NewState = existingPromo
        });

        return existingPromo;
    }

    private List<Product> GetValidatedProducts(IEnumerable<string> productCodes)
    {
        var codes = productCodes.ToList();
        var selectedProducts = _productRepository.GetAll()
            .Where(p => codes.Contains(p.Code))
            .ToList();

        if(selectedProducts.Count != codes.Count)
        {
            throw new ArgumentException("Uno o más códigos de producto no son válidos.");
        }

        return selectedProducts;
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
