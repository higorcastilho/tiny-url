using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyUrl.Data.Context
{
    public class DistributedCacheContext
    {
        public static ConnectionMultiplexer connection;

        public DistributedCacheContext()
        {
            connection = LazyConnection.Value;
        }

        private static Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            ConfigurationOptions configuration = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                ConnectTimeout = 5000,
            };

            configuration.EndPoints.Add("redis", 6379);

            return ConnectionMultiplexer.Connect(configuration.ToString());

        });
    }
}
