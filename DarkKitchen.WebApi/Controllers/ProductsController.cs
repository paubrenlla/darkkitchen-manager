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
        return Ok(products);
    }
}
