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

    [HttpGet]
    public IActionResult GetUsers(
        [FromQuery] string? name,
        [FromQuery] string? surname)
    {
        IEnumerable<UserCreateResponse> users = _userService.GetUsers(name, surname);
        return Ok(users);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(Guid id, [FromBody] UserUpdateRequest request, [FromHeader(Name = "X-Admin-Id")] Guid adminId)
    {
        try
        {
            UserCreateResponse response = _userService.UpdateUser(adminId, id, request);
            return Ok(response);
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
