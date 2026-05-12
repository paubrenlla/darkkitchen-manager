using System.Collections.Generic;
using System.Linq;
using DarkKitchen.Plugin.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PluginsController : ControllerBase
{
    private readonly IEnumerable<IProductImporter> _importers;

    public PluginsController(IEnumerable<IProductImporter> importers)
    {
        _importers = importers;
    }

    [HttpGet("importers")]
    public IActionResult GetImporters()
    {
        var importerNames = _importers.Select(i => i.Name).ToList();
        return Ok(importerNames);
    }
}
