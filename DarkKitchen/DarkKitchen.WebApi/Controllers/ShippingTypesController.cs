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
        var types = _shippingTypeService.GetAll().ToList();
        return types.Any() ? Ok(types) : NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Administrativo")]
    public IActionResult Create([FromBody] ShippingTypeRequest request)
    {
        try
        {
            var response = _shippingTypeService.Create(request);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult Update(Guid id, [FromBody] ShippingTypeRequest request)
    {
        try
        {
            var response = _shippingTypeService.Update(id, request);
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

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            _shippingTypeService.Delete(id);
            return NoContent();
        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
