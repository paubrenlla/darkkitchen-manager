using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
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
        IEnumerable<ProductResponse> products = _productService.GetProducts(name, line, category);
        return products.Any() ? Ok(products) : NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Administrativo")]
    public IActionResult CreateProduct([FromBody] ProductCreateRequest request)
    {
        return StatusCode(StatusCodes.Status201Created, _productService.CreateProduct(request));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult UpdateProduct(Guid id, [FromBody] ProductUpdateRequest request)
    {
        return Ok(_productService.UpdateProduct(id, request));
    }
}
