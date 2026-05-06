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
        var promotions = _promotionService.GetPromotions(date, line, productCode);
        if(!promotions.Any())
        {
            return NoContent();
        }

        return Ok(promotions);
    }

    [HttpPost]
    [Authorize(Roles = "Administrativo")]
    public IActionResult CreatePromotion([FromBody] PromotionCreateRequest request)
    {
        var currentUser = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Unknown";
        return StatusCode(StatusCodes.Status201Created, _promotionService.CreatePromotion(request, currentUser));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult UpdatePromotion(Guid id, [FromBody] PromotionCreateRequest request)
    {
        var currentUser = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Unknown";
        return Ok(_promotionService.UpdatePromotion(id, request, currentUser));
    }
}
