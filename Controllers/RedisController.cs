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
    [Route("/redis/ping")]
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



    [HttpGet()]
    [Route("/redis/insert_test/{qty_keys_to_insert}")]
    public ActionResult GetStressTest(int qty_keys_to_insert)
    {
        if (qty_keys_to_insert > 120_000)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { Message = $"The value {qty_keys_to_insert} is too big. Try a value less than 120000." });
        }

        bool isRedisOk = _redisService.IsRedisOk();
        if (!isRedisOk)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, new { Message = "Operation canceled. Redis is responding slowly (timeout) or it is offline." });
        }


        DateTime startTime = DateTime.Now;
        string returnText = $"Inserting {qty_keys_to_insert} key-pairs in Redis. ";

        string[] entryKey = new string[qty_keys_to_insert];
        string[] entryValue = new string[qty_keys_to_insert];
        string keyPrefix = DateTime.Now.ToFileTimeUtc().ToString();

        for (int i = 0; i < qty_keys_to_insert; i++)
        {
            entryKey[i] = "key-test-" + keyPrefix + "-" + i.ToString();
            entryValue[i] = "sample-test-value-" + Guid.NewGuid().ToString();
        }
        TimeSpan totalArrayCreationTime = DateTime.Now - startTime;
        returnText += $"Sample Array created in {totalArrayCreationTime.TotalMilliseconds} ms. ";



        startTime = DateTime.Now;
        for (int i = 0; i < qty_keys_to_insert; i++)
        {
            _redisService.SetCache(entryKey[i], entryValue[i], 60, CommandFlags.FireAndForget);
        }
        TimeSpan totalRedisInsertTime = DateTime.Now - startTime;
        returnText += $"Total Redis Insertions {totalRedisInsertTime.TotalMilliseconds} ms.";

        return Ok(new { Message = returnText });

    }





    [HttpGet()]
    [Route("/redis/insert_async_test/{qty_keys_to_insert}")]
    public async Task<ActionResult> GetStressTestAsync(int qty_keys_to_insert)
    {
        if (qty_keys_to_insert > 120_000)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { Message = $"The value {qty_keys_to_insert} is too big. Try a value less than 120000." });
        }

        bool isRedisOk = _redisService.IsRedisOk();
        if (!isRedisOk)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, new { Message = "Operation canceled. Redis is responding slowly (timeout) or it is offline." });
        }


        DateTime startTime = DateTime.Now;
        string returnText = $"Inserting {qty_keys_to_insert} key-pairs in Redis. ";

        string[] entryKey = new string[qty_keys_to_insert];
        string[] entryValue = new string[qty_keys_to_insert];
        string keyPrefix = DateTime.Now.ToFileTimeUtc().ToString();

        for (int i = 0; i < qty_keys_to_insert; i++)
        {
            entryKey[i] = $"key-test-{keyPrefix}-{i}";
            entryValue[i] = $"sample-test-value-{Guid.NewGuid().ToString()}";
        }
        TimeSpan totalArrayCreationTime = DateTime.Now - startTime;
        returnText += $"Sample Array created in {totalArrayCreationTime.TotalMilliseconds} ms. ";



        startTime = DateTime.Now;
        Task[] myTasks = new Task[qty_keys_to_insert];
        for (int i = 0; i < qty_keys_to_insert; i++)
        {
            myTasks[i] = _redisService.SetCacheAsync(entryKey[i], entryValue[i], 60); //async task
        }
        await Task.WhenAll(myTasks); //await for all async tasks
        TimeSpan totalRedisInsertTime = DateTime.Now - startTime;
        returnText += $"Total Redis Insertions {totalRedisInsertTime.TotalMilliseconds} ms.";

        //performing read test
        string lastRedisKey = entryKey[qty_keys_to_insert - 1];
        string? lastRedisValue = _redisService.GetCachedString(lastRedisKey);

        returnText += $"The last key is {lastRedisKey} and your value is {lastRedisValue}";

        return Ok(new { Message = returnText });

    }

    /// <summary>
    /// Adds a new job object in the queue. (Right push on queue)
    /// </summary>
    /// <param name="queue_id"></param>
    /// <param name="_object"></param>
    /// <returns></returns>
    [HttpPost()]
    [Route("/redis/queues/{queue_id}/enqueue")]
    public async Task<ActionResult<RedisQueueItem>> PostRedisEnqueue(string queue_id, RedisQueueItem _object)
    {


        bool isRedisOk = _redisService.IsRedisOk();
        if (!isRedisOk)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, new { Message = "Operation canceled. Redis is responding slowly (timeout) or it is offline." });
        }

        var result = await _redisService.EnqueueObject(queue_id, _object);
        if (!result)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, _object);
        }

        return Ok(_object);

    }

    /// <summary>
    /// Return the oldest job object in the queue, removing it from queue (Left pop from queue)
    /// </summary>
    /// <param name="queue_id"></param>
    /// <returns></returns>
    [HttpGet()]
    [Route("/redis/queues/{queue_id}/dequeue")]
    public async Task<ActionResult<RedisQueueItem>> GetRedisDequeue(string queue_id)
    {
        bool isRedisOk = _redisService.IsRedisOk();
        if (!isRedisOk)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, new { Message = "Operation canceled. Redis is responding slowly (timeout) or it is offline." });
        }

        var _object = await _redisService.DequeueObject(queue_id);

        return Ok(_object);

    }

    /// <summary>
    ///  Return all objects in the queue
    /// </summary>
    /// <param name="queue_id"></param>
    /// <param name="max_qty">Max quantity of returning objects. Default: 50</param>
    /// <returns></returns>
    [HttpGet()]
    [Route("/redis/queues/{queue_id}")]
    public async Task<ActionResult<List<RedisQueueItem>>> GetRedisReturnAllQueueObjects(string queue_id, [FromQuery] int max_qty = 50)
    {
        bool isRedisOk = _redisService.IsRedisOk();
        if (!isRedisOk)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, new { Message = "Operation canceled. Redis is responding slowly (timeout) or it is offline." });
        }

        var _objects = await _redisService.ReturnAllQueueObjects(queue_id, max_qty);
        return Ok(_objects);

    }




    [HttpDelete()]
    [Route("/redis/queues/{queue_id}/{index}")]
    public async Task<ActionResult> DeleteRedisQueuedItem(string queue_id, int index)
    {
        bool isRedisOk = _redisService.IsRedisOk();
        if (!isRedisOk)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, new { Message = "Operation canceled. Redis is responding slowly (timeout) or it is offline." });
        }

        bool _result = await _redisService.DeleteQueuedItem(queue_id, index);
        if (!_result)
        {
            return NotFound();

        }

        return Ok();

    }

    /// <summary>
    /// Returns all queue names in use
    /// </summary>
    /// <param name="max_qty"></param>
    /// <returns></returns>
    [HttpGet()]
    [Route("/redis/queues")]
    public async Task<ActionResult<List<string>>> GetRedisReturnAllQueues(  [FromQuery] int max_qty = 50)
    {
        bool isRedisOk = _redisService.IsRedisOk();
        if (!isRedisOk)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, new { Message = "Operation canceled. Redis is responding slowly (timeout) or it is offline." });
        }

        var _objects = await _redisService.ReturnAllQueues( max_qty);
        return Ok(_objects);

    }


}
