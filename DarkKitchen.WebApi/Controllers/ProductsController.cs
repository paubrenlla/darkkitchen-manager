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
        var products = _productService.GetProducts(name, line, category);
        if(!products.Any())
        {
            return NoContent();
        }

        return Ok(products);
    }

    [HttpPost]
    [Authorize(Roles = "Administrativo")]
    public IActionResult CreateProduct([FromBody] ProductCreateRequest request)
    {
        try
        {
            ProductResponse response = _productService.CreateProduct(request);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult UpdateProduct(Guid id, [FromBody] ProductUpdateRequest request)
    {
        try
        {
            ProductResponse response = _productService.UpdateProduct(id, request);
            return Ok(response);
        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
