using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost]
    public IActionResult CreateUser([FromBody] UserCreateRequest request)
    {
        try
        {
            var response = _userService.CreateUser(request);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
