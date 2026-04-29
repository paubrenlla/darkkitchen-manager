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
        return Ok(_promotionService.GetPromotions(date, line, productCode));
    }

    [HttpPost]
    [Authorize(Roles = "Administrativo")]
    public IActionResult CreatePromotion([FromBody] PromotionCreateRequest request)
    {
        return StatusCode(StatusCodes.Status201Created, _promotionService.CreatePromotion(request));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult UpdatePromotion(Guid id, [FromBody] PromotionCreateRequest request)
    {
        return Ok(_promotionService.UpdatePromotion(id, request));
    }
}
