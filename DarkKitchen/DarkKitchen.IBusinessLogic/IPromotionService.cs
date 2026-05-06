using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IPromotionService
{
    IEnumerable<PromotionCreateResponse> GetPromotions(DateTime? date, string? line, string? productCode);
    PromotionCreateResponse CreatePromotion(PromotionCreateRequest request, string responsibleUser);
    PromotionCreateResponse UpdatePromotion(Guid id, PromotionCreateRequest request, string responsibleUser);
    (string? PromotionName, decimal Discount) GetBestPromotionForProduct(Guid productId, DateTime date);
}
