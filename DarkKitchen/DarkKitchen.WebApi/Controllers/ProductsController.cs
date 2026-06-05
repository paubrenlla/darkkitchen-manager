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
        var products = _productService.GetProducts(name, line, category)
            .Select(p => new ProductResponse(p))
            .ToList();
        return products.Any() ? Ok(products) : NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Administrativo")]
    public IActionResult CreateProduct([FromBody] ProductCreateRequest request)
    {
        var currentUser = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Unknown";
        var product = _productService.CreateProduct(request, currentUser);
        return StatusCode(StatusCodes.Status201Created, new ProductResponse(product));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult UpdateProduct(Guid id, [FromBody] ProductUpdateRequest request)
    {
        var currentUser = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Unknown";
        var product = _productService.UpdateProduct(id, request, currentUser);
        return Ok(new ProductResponse(product));
    }

    [HttpPost("import")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult ImportProducts([FromBody] ProductImportRequest request)
    {
        var currentUser = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Unknown";
        var results = _productService.ImportProducts(request.ImporterName, request.FilePath, currentUser);
        return StatusCode(StatusCodes.Status201Created, results);
    }
}
