using DarkKitchen.IBusinessLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [HttpGet]
    public IActionResult GetProducts(
        [FromQuery] string? name,
        [FromQuery] string? line,
        [FromQuery] string? category)
    {
        var products = _productService.GetProducts(name, line, category);

        var result = products.Select(p => new
        {
            p.Code,
            p.Name,
            p.Description,
            p.Price,
            Line = p.Line.Name,
            Category = p.Category.Name,
        });

        return Ok(result);
    }
}
