using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TinyUrl.Data.Interface;
using TinyUrl.Domain.Entity;
using TinyUrl.Framework.Converters;
using TinyUrl.Service.Interface;

namespace TinyUrl.Service.Services
{
    public class LinkService : ILinkService
    {
        private readonly ICacheRepository _cacheRepository;
        public LinkService(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }
        public async Task<Link> Generate(string longUrl)
        {
            var containerId = Dns.GetHostName();
            var counter = _cacheRepository.Get(containerId);

            var shortUrl = Base10ToBase62.Convert(int.Parse(counter));
            var link = new Link
            {
                LongUrl = longUrl,
                ShortUrl = shortUrl
            };
            return link;
        }
    }
}
