using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using StackExchange.Redis;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly RedisCacheService _redisService;
    public WeatherForecastController(ILogger<WeatherForecastController> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redisService = new RedisCacheService(redis);
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {


        //Try to get weatherForecast from redis cache
        var cacheKey = "my_city_forecast";
        var cachedList = _redisService.GetCachedList<WeatherForecast>(cacheKey);
        if (cachedList.Count > 0)
        {
            //return from cache
            return cachedList;
        }




        //create a ffake Weather forecast List
        var resultForecast = Enumerable.Range(1, 10).Select(index => new WeatherForecast
        {
            date = DateTime.Now.AddDays(index),
            temperatureC = Random.Shared.Next(-20, 55),
            summary = summaries[Random.Shared.Next(summaries.Length)]
        }).ToArray();


        //store List in cache for 30 seconds
        _redisService.SetCache(cacheKey, resultForecast, 30);

        //return List
        return resultForecast;
    }

}
