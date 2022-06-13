using Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Services
{
    public class RedisCacheService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public void SetCache(string cacheKey, Object objectToCache, int cacheSeconds)
        {

            var jsonObject = JsonSerializer.Serialize(objectToCache, objectToCache.GetType());
            SetCache(cacheKey, jsonObject, cacheSeconds);


        }
        public void SetCache(string cacheKey, string? stringToCache, int cacheSeconds, CommandFlags _flags = CommandFlags.PreferMaster)
        {
            try
            {
                var redisDb = _redis.GetDatabase();
                var expire = new TimeSpan(0, 0, cacheSeconds);
                redisDb.StringSet(cacheKey, stringToCache, expire, When.Always, flags: _flags);
            }
            catch
            {
                //Do nothing. Just to protect against timeout or redis offline
            }
        }
        public async Task SetCacheAsync(string cacheKey, string? stringToCache, int cacheSeconds, CommandFlags _flags = CommandFlags.PreferMaster)
        {
            try
            {
                var redisDb = _redis.GetDatabase();
                var expire = new TimeSpan(0, 0, cacheSeconds);
                await redisDb.StringSetAsync(cacheKey, stringToCache, expire, When.Always, flags: _flags);
            }
            catch
            {
                //Do nothing. Just to protect against timeout or redis offline
            }
        }
        public List<T> GetCachedList<T>(string cacheKey)
        {

            try
            {

                var redisDb = _redis.GetDatabase();
                var jsonObject = redisDb.StringGet(cacheKey).ToString();

                if (jsonObject == null)
                {
                    return new List<T>(); //returns an empty List<T>
                }

                //Deserialize jsonObject to List
                var myObjList = System.Text.Json.JsonSerializer.Deserialize<List<T>>(jsonObject);
                return myObjList;
            }
            catch
            {
                return new List<T>(); //return an Empty List if Redis is offline
            }
        }
        public string? GetCachedString(string cacheKey)
        {

            try
            {

                var redisDb = _redis.GetDatabase();
                var keyValue = redisDb.StringGet(cacheKey).ToString();
                return keyValue;
            }
            catch
            {
                return null;
            }
        }


        public async Task<bool> EnqueueObject(string queueId, RedisQueueItem _job)
        {
            try
            {
                var redisDb = _redis.GetDatabase();
                long positionInserted = await redisDb.ListRightPushAsync(queueId, System.Text.Json.JsonSerializer.Serialize(_job));
                await redisDb.ListGetByIndexAsync(queueId, positionInserted);
                return true;
            }
            catch
            {
                //Do nothing. Just to protect against timeout or redis offline
                return false;
            }
        }
        public async Task<RedisQueueItem> DequeueObject(string queueId)
        {
            try
            {
                var redisDb = _redis.GetDatabase();
                // var expire = new TimeSpan(0, 0, cacheSeconds);
                var obj = await redisDb.ListLeftPopAsync(queueId);
                if (string.IsNullOrEmpty(obj))
                {
                    return null; //nothing in the queue to be dequeued
                }
                return JsonSerializer.Deserialize<RedisQueueItem>(obj);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<RedisQueueItem>> ReturnAllQueueObjects(string queueId, int maxQty)
        {
            var listObjects = new List<RedisQueueItem>();

            try
            {
                var redisDb = _redis.GetDatabase();
                long listCount = await redisDb.ListLengthAsync(queueId);
                if (listCount > maxQty)
                {
                    listCount = maxQty; //to return only the max_qty
                }

                for (long i = 0; i < listCount; i++)
                {
                    var item = await redisDb.ListGetByIndexAsync(queueId, i);
                    var itemObj = JsonSerializer.Deserialize<RedisQueueItem>(item);
                    listObjects.Add(itemObj);
                }
            }
            catch
            {
                return null;
            }

            return listObjects;

        }

        public async Task<bool> DeleteQueuedItem(string queueId, long index)
        {

            try
            {
                var redisDb = _redis.GetDatabase();
                var listCountBeforeDeletion = await redisDb.ListLengthAsync(queueId);
                if (index > listCountBeforeDeletion - 1) return false; //if index greather than List Length, exit

                //find element by index
                var listElem = await redisDb.ListGetByIndexAsync(queueId, index);
                if (listElem == RedisValue.Null) return false; //element index not found

                //remove the element
                await redisDb.ListRemoveAsync(queueId, listElem);

                //compare list count after deletion
                var listCountAfterDeletion = await redisDb.ListLengthAsync(queueId);

                //return success
                return listCountAfterDeletion == listCountBeforeDeletion - 1;
            }
            catch
            {
                return false;
            }

        }


        public async Task<List<string>> ReturnAllQueues(int maxQty)
        {
            var listKeys = new List<string>();

            try
            {
                //var redisServer = _redis.GetServer(Microsoft.Extensions.Configuration.GetConnectionString("MyRedisConStr"));
                var redisServer = _redis.GetServer("localhost", 6379);

                var keys = redisServer.Keys();
                listKeys.AddRange(keys.Select(key => (string)key).ToList());

            }
            catch
            {
                //do nothing
            }
            return listKeys;

        }
        public bool IsRedisOk()
        {

            bool redisState = true;
            try
            {
                var redisDb = _redis.GetDatabase();
                var pong = redisDb.Ping();

                //response time > 1000 millisec must set redisState = false
                if (pong.TotalMilliseconds > 1000)
                {
                    redisState = false; // if redis is slow, set redisState = false
                }
            }
            catch
            {
                redisState = false;
            }

            return redisState;

        }

        public double ReturnRedisResponseTime()
        {

            try
            {
                var redisDb = _redis.GetDatabase();
                var pong = redisDb.Ping();
                return pong.TotalMilliseconds;
            }
            catch
            {
                return -1; //returning -1 in case of error
            }
        }


    }
}
