using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShippingTypesController(IShippingTypeService shippingTypeService) : ControllerBase
{
    private readonly IShippingTypeService _shippingTypeService = shippingTypeService;

    [HttpGet]
    public IActionResult GetAll()
    {
        var types = _shippingTypeService.GetAll()
            .Select(s => new ShippingTypeResponse(s))
            .ToList();
        return Ok(types);
    }

    [HttpPost]
    [Authorize(Roles = "Administrativo")]
    public IActionResult Create([FromBody] ShippingTypeRequest request)
    {
        var shippingType = _shippingTypeService.Create(request);
        return StatusCode(StatusCodes.Status201Created, new ShippingTypeResponse(shippingType));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult Update(Guid id, [FromBody] ShippingTypeRequest request)
    {
        var shippingType = _shippingTypeService.Update(id, request);
        return Ok(new ShippingTypeResponse(shippingType));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult Delete(Guid id)
    {
        _shippingTypeService.Delete(id);
        return NoContent();
    }
}
