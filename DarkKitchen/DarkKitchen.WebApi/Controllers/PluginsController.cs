using DarkKitchen.Plugin.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PluginsController(IEnumerable<IProductImporter> importers) : ControllerBase
{
    private readonly IEnumerable<IProductImporter> _importers = importers;

    [HttpGet("importers")]
    public IActionResult GetImporters()
    {
        var importerNames = _importers.Select(i => i.Name).ToList();
        return Ok(importerNames);
    }
}
