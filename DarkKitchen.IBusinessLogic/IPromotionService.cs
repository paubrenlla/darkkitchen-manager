using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IPromotionService
{
    IEnumerable<PromotionCreateResponse> GetPromotions(DateTime? date, string? line, string? productCode);
    PromotionCreateResponse CreatePromotion(PromotionCreateRequest request);
}
