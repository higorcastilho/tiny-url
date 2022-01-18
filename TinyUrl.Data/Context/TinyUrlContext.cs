using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyUrl.Domain.Entity;

namespace TinyUrl.Data.Context
{
    public class TinyUrlContext : ITinyUrlContext
    {
        public IMongoCollection<Link> Links { get; }

        public TinyUrlContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            Links = database.GetCollection<Link>("Links");
            ;
        }
    }
}
