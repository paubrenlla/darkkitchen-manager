using DarkKitchen.Domain.Promotions;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IPromotionService
{
    IEnumerable<Promotion> GetPromotions(DateTime? date, string? line, string? productCode);
    Promotion CreatePromotion(PromotionCreateRequest request, string responsibleUser);
    Promotion UpdatePromotion(Guid id, PromotionCreateRequest request, string responsibleUser);
    (string? PromotionName, decimal Discount) GetBestPromotionForProduct(Guid productId, DateTime date);
}
