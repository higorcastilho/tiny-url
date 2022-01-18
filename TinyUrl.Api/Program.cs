using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TinyUrl.Api
{
    public class Program
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            ConfigurationOptions configuration = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                ConnectTimeout = 5000,
            };

            configuration.EndPoints.Add("redis", 6379);

            return ConnectionMultiplexer.Connect(configuration.ToString());

        });

        public static ConnectionMultiplexer Connection => lazyConnection.Value;

        static bool AcquiredLock(string key, string value, TimeSpan expiration)
        {
            bool flag = false;
            try
            {
                flag = Connection.GetDatabase().StringSet(key, value, expiration, When.NotExists);
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
                var res = Connection.GetDatabase().ScriptEvaluate(lua_script, new RedisKey[] { key }, new RedisValue[] { value });
                return (bool)res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReleaseLock lock fail... {ex.Message}");
                return false;
            }
        }

        public static void Main(string[] args)
        {
            var range = 10;
            var containerId  = Dns.GetHostName();

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
                var counter = Connection.GetDatabase().StringGet(containerId);

                if (counter.IsNull)
                {
                    var rangeStart = Connection.GetDatabase().StringGet("rangeStart");
                    
                    if (rangeStart.IsNull)
                    {
                        Connection.GetDatabase().StringSet("rangeStart", "0");
                    }

                    rangeStart = Connection.GetDatabase().StringGet("rangeStart");

                    counter = rangeStart.ToString();
                    Connection.GetDatabase().StringSet(containerId, counter);

                    Connection.GetDatabase().StringSet("rangeStart", $"{int.Parse(rangeStart) + 10}");

                    Console.WriteLine($"{containerId} - {Connection.GetDatabase().StringGet(containerId)}");
                }
                
                //Console.WriteLine($"{person} begin eat food(with lock) at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}.");
                //if (new Random().NextDouble() < 0.6)
                //{
                //    Console.WriteLine($"{person} release lock {ReleaseLock(lockKey, person)} {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
                //}
                //else
                //{
                //    Console.WriteLine($"{person} do not release lock ...");
                //}
            }
            else
            {
                
                //Console.WriteLine($"{person} begin eat food(without lock) at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}.");
            }


            //string lockKey = "lock:eat";
            //TimeSpan expiration = TimeSpan.FromSeconds(5);
            
            //Parallel.For(0, 5, x => {
            //    string person = $"person{x}";
            //    var val = 0;
            //    bool isLocked = AcquiredLock(lockKey, person, expiration);
            //    while (!isLocked && val <= 5000)
            //    {
            //        val += 250;
            //        System.Threading.Thread.Sleep(250);
            //        isLocked = AcquiredLock(lockKey, person, expiration);
            //    }

            //    if (isLocked)
            //    {
            //        Console.WriteLine($"{person} begin eat food(with lock) at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}.");
            //        if (new Random().NextDouble() < 0.6)
            //        {
            //            Console.WriteLine($"{person} release lock {ReleaseLock(lockKey, person)} {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
            //        }
            //        else
            //        {
            //            Console.WriteLine($"{person} do not release lock ...");
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine($"{person} begin eat food(without lock) at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}.");
            //    }
            //});

            //string lockKey = "lock:eat";
            //TimeSpan expiration = TimeSpan.FromSeconds(5);
            //Parallel.For(0, 5, x => {
            //    string person = $"person:{x}";
            //    bool isLocked = AcquiredLock(lockKey, person, expiration);

            //    if (isLocked)
            //    {
            //        Console.WriteLine($"{person} begin eat food(with lock) at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}.");
            //    }
            //    else
            //    {
            //        Console.WriteLine($"{person} can not eat food due to don't get the lock.");
            //    }
            //});

            //Console.WriteLine("end");
            //Console.Read();

            CreateHostBuilder(args).Build().Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                });
    }
}
