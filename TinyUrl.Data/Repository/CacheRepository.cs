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

        public string Get(string key)
        {
            var result = _connectionMultiplexer.GetDatabase().StringGet(key);
            return result;
        }

        public void Set(string key, string value)
        {
            _connectionMultiplexer.GetDatabase().StringSet(key, value);
        }

        public bool SetWithExpirationTime(string key, string value, TimeSpan expiration)
        {
            var result = _connectionMultiplexer.GetDatabase().StringSet(key, value, expiration, When.NotExists);
            return (bool)result;
        }

        public bool ScriptEvaluate(string script, string key, string value)
        {
            var result = _connectionMultiplexer.GetDatabase().ScriptEvaluate(script, new RedisKey[] { key }, new RedisValue[] { value });
            return (bool)result;
        }


    }
}
