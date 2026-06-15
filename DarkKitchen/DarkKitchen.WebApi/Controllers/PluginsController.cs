using DarkKitchen.IBusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PluginsController(IPluginService pluginService) : ControllerBase
{
    private readonly IPluginService _pluginService = pluginService;

    [HttpGet("importers")]
    public IActionResult GetImporters()
    {
        var names = _pluginService.GetAvailableImporters().ToList();
        return Ok(names);
    }
}
