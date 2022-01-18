using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyUrl.Domain.Entity;

namespace TinyUrl.Data.Context
{
    public interface ITinyUrlContext
    {
        IMongoCollection<Link> Links { get; }
    }
}
