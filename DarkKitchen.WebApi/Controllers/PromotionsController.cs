using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromotionsController(IPromotionService promotionService) : ControllerBase
{
    private readonly IPromotionService _promotionService = promotionService;

    [HttpGet]
    public IActionResult GetPromotions(
        [FromQuery] DateTime? date,
        [FromQuery] string? line,
        [FromQuery] string? productCode)
    {
        IEnumerable<PromotionCreateResponse> result = _promotionService.GetPromotions(date, line, productCode);
        return Ok(result);
    }
}
