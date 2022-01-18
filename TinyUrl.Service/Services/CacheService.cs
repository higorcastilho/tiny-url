using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TinyUrl.Data.Interface;
using TinyUrl.Service.Interface;

namespace TinyUrl.Service.Services
{
    public class CacheService : ICacheService
    {
        private readonly IConfiguration _configuration;
        private static ICacheRepository _repository;
        public CacheService(IConfiguration configuration, ICacheRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        static async Task<bool> AcquiredLock(string key, string value, TimeSpan expiration)
        {
            bool flag = false;
            try
            {
                flag = await _repository.SetWithExpirationTime(key, value, expiration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Acquire lock fail...{ex.Message}");
                flag = true;
            }

            return flag;
        }

        static async Task<bool> ReleaseLock(string key, string value)
        {
            string lua_script = @"
                if (redis.call('GET', KEYS[1]) == ARGV[1]) then
                    redis.call('DEL', KEYS[1])
                    return true
                else
                    return false
                end
            ";
            try
            {
                var res = await _repository.ScriptEvaluate(lua_script, key , value);
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReleaseLock lock fail... {ex.Message}");
                return false;
            }
        }

        public async Task GetRange()
        {
            var range = _configuration.GetValue<long>("Range");
            var containerId = Dns.GetHostName();

            string lockKey = "lock:range";
            TimeSpan expiration = TimeSpan.FromSeconds(5);

            var val = 0;
            bool isLocked = await AcquiredLock(lockKey, containerId, expiration);

            var rnd = new Random();

            while (!isLocked && val <= 5000)
            {
                val += 250;
                System.Threading.Thread.Sleep(rnd.Next(250, 3000));
                isLocked = await AcquiredLock(lockKey, containerId, expiration);
            }

            if (isLocked)
            {
                var containerRangeCounter = await _repository.Get(containerId);

                var containerRageCurrent = containerRangeCounter?.ToString()?.Split("-")[0];
                var containerRangeMax = containerRangeCounter?.ToString()?.Split("-")[1];

                if (containerRangeCounter == null || (containerRageCurrent == containerRangeMax))
                {
                    var rangeStart = await _repository.Get("rangeStart");

                    if (rangeStart == null)
                    {
                        await _repository.Set("rangeStart", "0");
                    }

                    rangeStart = await _repository.Get("rangeStart");

                    containerRangeCounter = $"{long.Parse(rangeStart)}-{long.Parse(rangeStart) + range}";
                    await _repository.Set(containerId, containerRangeCounter);

                    await _repository.Set("rangeStart", $"{int.Parse(rangeStart) + range}");

                    Console.WriteLine($"{containerId} - {await _repository.Get(containerId)}");

                    await ReleaseLock(lockKey, containerId);
                }
            }
        }
    }
}
