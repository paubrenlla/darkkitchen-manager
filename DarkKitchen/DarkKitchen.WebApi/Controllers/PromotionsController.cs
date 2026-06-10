using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PromotionsController(IPromotionService promotionService) : ControllerBase
{
    private readonly IPromotionService _promotionService = promotionService;

    [HttpGet]
    [Authorize(Roles = "Cliente,Administrativo")]
    public IActionResult GetPromotions(
        [FromQuery] DateTime? date,
        [FromQuery] string? line,
        [FromQuery] string? productCode)
    {
        if (date.HasValue && date.Value.TimeOfDay == TimeSpan.Zero)
        {
            date = date.Value.Date.AddDays(1).AddSeconds(-1);
        }

        var promotions = _promotionService.GetPromotions(date, line, productCode)
            .Select(p => new PromotionCreateResponse(p))
            .ToList();
        return Ok(promotions);
    }

    [HttpPost]
    [Authorize(Roles = "Administrativo")]
    public IActionResult CreatePromotion([FromBody] PromotionCreateRequest request)
    {
        var currentUser = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Unknown";
        var promotion = _promotionService.CreatePromotion(request, currentUser);
        return StatusCode(StatusCodes.Status201Created, new PromotionCreateResponse(promotion));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult UpdatePromotion(Guid id, [FromBody] PromotionCreateRequest request)
    {
        var currentUser = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Unknown";
        var promotion = _promotionService.UpdatePromotion(id, request, currentUser);
        return Ok(new PromotionCreateResponse(promotion));
    }
}
