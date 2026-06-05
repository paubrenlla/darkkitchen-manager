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
        if(request.Role != null)
        {
            var callerRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if(callerRole != "Administrativo")
            {
                return Forbid();
            }
        }

        var user = _userService.CreateUser(request);
        return StatusCode(StatusCodes.Status201Created, new UserCreateResponse(user));
    }

    [HttpGet]
    [Authorize(Roles = "Administrativo")]
    public IActionResult GetUsers([FromQuery] string? name, [FromQuery] string? surname)
    {
        var users = _userService.GetUsers(name, surname)
            .Select(u => new UserCreateResponse(u))
            .ToList();
        return users.Any() ? Ok(users) : NoContent();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult UpdateUser(Guid id, [FromBody] UserUpdateRequest request)
    {
        var callerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = _userService.UpdateUser(callerId, id, request);
        return Ok(new UserCreateResponse(user));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrativo")]
    public IActionResult DeleteUser(Guid id)
    {
        var callerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        _userService.DeleteUser(callerId, id);
        return NoContent();
    }
}
