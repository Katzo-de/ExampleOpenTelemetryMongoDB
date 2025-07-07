using ExampleOpenTelemetryMongoDB.Business;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ExampleOpenTelemetryMongoDB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        private readonly WeatherForecastBusiness _business;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(WeatherForecastBusiness business, ILogger<WeatherForecastController> logger)
        {
            _business = business;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            _logger.LogInformation("Fetching weather forecasts from MongoDB");
            return await _business.GetForecastAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WeatherForecast forecast)
        {
            _logger.LogInformation("Inserting weather forecast: {@Forecast}", forecast);
            await _business.InsertForecastAsync(forecast);
            return Ok();
        }

    }
}
