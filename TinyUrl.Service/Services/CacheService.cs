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
        private static ICacheRepository _repository;
        public CacheService(ICacheRepository repository)
        {
            _repository = repository;
        }

        static bool AcquiredLock(string key, string value, TimeSpan expiration)
        {
            bool flag = false;
            try
            {
                flag = _repository.SetWithExpirationTime(key, value, expiration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Acquire lock fail...{ex.Message}");
                flag = true;
            }

            return flag;
        }

        static bool ReleaseLock(string key, string value)
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
                var res = _repository.ScriptEvaluate(lua_script, key , value);
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReleaseLock lock fail... {ex.Message}");
                return false;
            }
        }

        public void GetRange()
        {
            var range = 10;
            var containerId = Dns.GetHostName();

            string lockKey = "lock:range";
            TimeSpan expiration = TimeSpan.FromSeconds(5);

            var val = 0;
            bool isLocked = AcquiredLock(lockKey, containerId, expiration);

            var rnd = new Random();

            while (!isLocked && val <= 5000)
            {
                val += 250;
                System.Threading.Thread.Sleep(rnd.Next(250, 3000));
                isLocked = AcquiredLock(lockKey, containerId, expiration);
            }

            if (isLocked)
            {
                var counter = _repository.Get(containerId);

                if (counter == null)
                {
                    var rangeStart = _repository.Get("rangeStart");

                    if (rangeStart == null)
                    {
                        _repository.Set("rangeStart", "0");
                    }

                    rangeStart = _repository.Get("rangeStart");

                    counter = rangeStart.ToString();
                    _repository.Set(containerId, counter);

                    _repository.Set("rangeStart", $"{int.Parse(rangeStart) + 10}");

                    Console.WriteLine($"{containerId} - {_repository.Get(containerId)}");

                    ReleaseLock(lockKey, containerId);
                }
            }
        }
    }
}
