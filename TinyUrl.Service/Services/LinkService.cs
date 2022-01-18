using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly ICacheRepository _cacheRepository;
        private readonly ICacheService _cacheService;
        public LinkService(IConfiguration configuration, ICacheRepository cacheRepository, ICacheService cacheService)
        {
            _configuration = configuration;
            _cacheRepository = cacheRepository;
            _cacheService = cacheService;
        }

        public string GetShortenedUrlRedirect(string shortUrl)
        {
            var longUrl = _cacheRepository.Get(shortUrl);
            return longUrl;
        }

        public string Generate(string longUrl, string host)
        {
            var containerId = Dns.GetHostName();
            var counter = _cacheRepository.Get(containerId);

            var range = _configuration.GetValue<long>("Range");
            if (int.Parse(counter) > 0 && (int.Parse(counter) % range) == 0)
            {
                _cacheService.GetRange();
                counter = _cacheRepository.Get(containerId);
            }

            var shortUrl = Base10ToBase62.Convert(int.Parse(counter));
            var link = new Link
            {
                LongUrl = longUrl,
                ShortUrl = shortUrl
            };

            _cacheRepository.Set(containerId, $"{int.Parse(counter) + 1}");
            _cacheRepository.Set(shortUrl, longUrl);

            //montar link com o meu host
            var generatedLink = $"{host}/{shortUrl}";

            return generatedLink;
        }
    }
}
