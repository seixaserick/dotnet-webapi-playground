using StackExchange.Redis;
using Models;
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
        public void SetCache(string cacheKey, string? stringToCache, int cacheSeconds)
        {

            try
            {
                var redisDb = _redis.GetDatabase();
                var expire = new TimeSpan(0, 0, cacheSeconds);
                redisDb.StringSet(cacheKey, stringToCache, expire);
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
                    redisState= false; // if redis is slow, set redisState = false
                }
            }
            catch
            {
                redisState=false;
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
