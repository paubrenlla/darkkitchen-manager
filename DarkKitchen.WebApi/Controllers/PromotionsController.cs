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
        var result = _promotionService.GetPromotions(date, line, productCode);
        return Ok(result);
    }

    [HttpPost]
    public IActionResult CreatePromotion([FromBody] PromotionCreateRequest request)
    {
        try
        {
            var response = _promotionService.CreatePromotion(request);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
