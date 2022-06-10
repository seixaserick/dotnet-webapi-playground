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

            var redisDb = _redis.GetDatabase();
            var jsonObject = JsonSerializer.Serialize(objectToCache, objectToCache.GetType());
            var expire = new TimeSpan(0, 0, cacheSeconds);
            redisDb.StringSet(cacheKey, jsonObject, expire);

        }
        public void SetCache(string cacheKey, string? stringToCache, int cacheSeconds)
        {

            SetCache(cacheKey, stringToCache, cacheSeconds);

        }
        public List<T> GetCachedList<T>(string cacheKey)
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

    }
}
