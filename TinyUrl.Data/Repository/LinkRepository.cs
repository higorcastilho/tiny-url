using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyUrl.Data.Context;
using TinyUrl.Data.Interface;
using TinyUrl.Domain.Entity;

namespace TinyUrl.Data.Repository
{
    public class LinkRepository : ILinkRepository
    {
        private readonly ITinyUrlContext _context;
        public LinkRepository(ITinyUrlContext context)
        {
            _context = context;
        }
        public async Task<Link> GetByShortUrl(string shortUrl)
        {
            FilterDefinition<Link> filter = Builders<Link>.Filter.Eq(l => l.ShortUrl, shortUrl);
            return await _context.Links.Find(filter).FirstOrDefaultAsync();
        }

        public void Create(Link link)
        {
            _context.Links.InsertOne(link);
        }
    }
}
