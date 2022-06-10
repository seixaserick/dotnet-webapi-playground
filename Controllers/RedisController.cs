using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using StackExchange.Redis;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class RedisController : ControllerBase
{



    private readonly ILogger<WeatherForecastController> _logger;
    private readonly RedisCacheService _redisService;
    public RedisController(ILogger<WeatherForecastController> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redisService = new RedisCacheService(redis);
    }

    [HttpGet()]
    [Route("ping")]
    public ActionResult Get()
    {


        bool isRedisOk = _redisService.IsRedisOk();
        if (!isRedisOk)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, new { Message = "Redis is responding slowly (timeout) or it is offline" });

        }

        var redisResponseTime = (int)_redisService.ReturnRedisResponseTime();
        return Ok(new { Message = $"Redis responding perfectly. Response time {redisResponseTime} ms." });


    }

}
