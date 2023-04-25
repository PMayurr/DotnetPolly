using DotnetPolly.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetPolly.Controllers;

[ApiController]
[Route("[controller]/")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ExternalApiService _externalApiService;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ExternalApiService externalApiService)
    {
        _logger = logger;
        this._externalApiService = externalApiService;
    }

    [HttpGet]
    [Route("")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet]
    [Route("GetApiResult")]
    public async Task<IActionResult> GetApiResult()
    {
        await _externalApiService.CallExternalAPIOnLoop();
        return Ok();
    }
}

