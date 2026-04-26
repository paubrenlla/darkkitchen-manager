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
        IEnumerable<PromotionCreateResponse> result = _promotionService.GetPromotions(date, line, productCode);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Administrativo")]
    public IActionResult CreatePromotion([FromBody] PromotionCreateRequest request)
    {
        try
        {
            PromotionCreateResponse response = _promotionService.CreatePromotion(request);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult UpdatePromotion(Guid id, [FromBody] PromotionCreateRequest request)
    {
        try
        {
            PromotionCreateResponse response = _promotionService.UpdatePromotion(id, request);
            return Ok(response);
        }
        catch(KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch(ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }
}
