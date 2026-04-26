using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IPromotionService
{
    IEnumerable<PromotionCreateResponse> GetPromotions(DateTime? date, string? line, string? productCode);
    PromotionCreateResponse CreatePromotion(PromotionCreateRequest request);
    PromotionCreateResponse UpdatePromotion(Guid id, PromotionCreateRequest request);
    (string? PromotionName, decimal Discount) GetBestPromotionForProduct(Guid productId, DateTime date);
}
