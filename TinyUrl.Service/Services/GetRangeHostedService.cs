using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyUrl.Data.Repository;

namespace TinyUrl.Service.Services
{
    public class GetRangeHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private static IConnectionMultiplexer _connection;

        public GetRangeHostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            
            using (var scope = _scopeFactory.CreateScope())
            {
                _connection = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            GetRange();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        static bool AcquiredLock(string key, string value, TimeSpan expiration)
        {
            bool flag = false;
            try
            {
                flag = _connection.GetDatabase().StringSet(key, value, expiration, When.NotExists);
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
                var res = _connection.GetDatabase().ScriptEvaluate(lua_script, new RedisKey[] { key }, new RedisValue[] { value });
                return (bool)res;
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
                var counter = _connection.GetDatabase().StringGet(containerId);

                if (counter.IsNull)
                {
                    var rangeStart = _connection.GetDatabase().StringGet("rangeStart");

                    if (rangeStart.IsNull)
                    {
                        _connection.GetDatabase().StringSet("rangeStart", "0");
                    }

                    rangeStart = _connection.GetDatabase().StringGet("rangeStart");

                    counter = rangeStart.ToString();
                    _connection.GetDatabase().StringSet(containerId, counter);

                    _connection.GetDatabase().StringSet("rangeStart", $"{int.Parse(rangeStart) + 10}");

                    Console.WriteLine($"{containerId} - {_connection.GetDatabase().StringGet(containerId)}");

                    ReleaseLock(lockKey, containerId);
                }
            }
        }
    }
}
