using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyUrl.Data.Interface;

namespace TinyUrl.Data.Repository
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        public CacheRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;   
        }

        public async Task<string> Get(string key)
        {
            var result = await _connectionMultiplexer.GetDatabase().StringGetAsync(key);
            return result;
        }

        public async Task Set(string key, string value)
        {
            await _connectionMultiplexer.GetDatabase().StringSetAsync(key, value);
        }

        public async Task<bool> SetWithExpirationTime(string key, string value, TimeSpan expiration)
        {
            var result = await _connectionMultiplexer.GetDatabase().StringSetAsync(key, value, expiration, When.NotExists);
            return (bool)result;
        }

        public async Task<bool> ScriptEvaluate(string script, string key, string value)
        {
            var result = await _connectionMultiplexer.GetDatabase().ScriptEvaluateAsync(script, new RedisKey[] { key }, new RedisValue[] { value });
            return (bool)result;
        }


    }
}
