using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController(IReportService reportService) : ControllerBase
{
    private readonly IReportService _reportService = reportService;

    [HttpGet("top-products")]
    public IActionResult GetTopProducts(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        if(fromDate > toDate)
        {
            return BadRequest(new { error = "La fecha de inicio no puede ser posterior a la fecha de fin." });
        }

        IEnumerable<TopProductResponse> topProducts = _reportService.GetTopProducts(fromDate, toDate);
        return Ok(topProducts);
    }
}
