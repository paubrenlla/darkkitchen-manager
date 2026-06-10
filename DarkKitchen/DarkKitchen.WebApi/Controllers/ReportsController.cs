using DarkKitchen.IBusinessLogic;
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
        if (toDate.TimeOfDay == TimeSpan.Zero)
        {
            toDate = toDate.Date.AddDays(1).AddSeconds(-1);
        }

        return Ok(_reportService.GetTopProducts(fromDate, toDate));
    }

    [HttpGet("sales")]
    public IActionResult GetSalesReport()
    {
        return Ok(_reportService.GetSalesReport());
    }
}
