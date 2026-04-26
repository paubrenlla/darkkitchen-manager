using System.Security.Claims;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost]
    [AllowAnonymous]
    public IActionResult CreateUser([FromBody] UserCreateRequest request)
    {
        try
        {
            if(request.Role != null)
            {
                var callerRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if(callerRole != "Administrativo")
                {
                    return Forbid();
                }
            }

            UserCreateResponse response = _userService.CreateUser(request);
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
    public IActionResult UpdateUser(Guid id, [FromBody] UserUpdateRequest request)
    {
        try
        {
            var callerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            UserCreateResponse response = _userService.UpdateUser(callerId, id, request);
            return Ok(response);
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(Guid id)
    {
        try
        {
            var callerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _userService.DeleteUser(callerId, id);
            return NoContent();
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
