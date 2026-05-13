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
        var currentUser = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Unknown";
        return StatusCode(StatusCodes.Status201Created, _productService.CreateProduct(request, currentUser));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult UpdateProduct(Guid id, [FromBody] ProductUpdateRequest request)
    {
        var currentUser = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Unknown";
        return Ok(_productService.UpdateProduct(id, request, currentUser));
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
