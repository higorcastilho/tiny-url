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

        public async Task<string> GetShortenedUrlRedirect(string shortUrl)
        {
            var longUrl = await _cacheRepository.Get(shortUrl);
            return longUrl;
        }

        public async Task<string> Generate(string longUrl, string host)
        {

            var containerId = Dns.GetHostName();

            var containerRangeCounter = await _cacheRepository.Get(containerId);
            var containerRageCurrent = containerRangeCounter?.ToString()?.Split("-")[0];
            var containerRangeMax = containerRangeCounter?.ToString()?.Split("-")[1];

            var range = _configuration.GetValue<long>("Range");
            if (long.Parse(containerRageCurrent) == long.Parse(containerRangeMax))
            {
                await _cacheService.GetRange();
                containerRangeCounter = await _cacheRepository.Get(containerId);
                containerRageCurrent = containerRangeCounter?.ToString()?.Split("-")[0];
                containerRangeMax = containerRangeCounter?.ToString()?.Split("-")[1];
            }

            var shortUrl = Base10ToBase62.Convert(long.Parse(containerRageCurrent));
            var link = new Link
            {
                LongUrl = longUrl,
                ShortUrl = shortUrl
            };

            await _cacheRepository.Set(containerId, $"{long.Parse(containerRageCurrent) + 1}-{containerRangeMax}");
            await _cacheRepository.Set(shortUrl, longUrl);

            //montar link com o meu host
            var generatedLink = $"{host}/{shortUrl}";

            return generatedLink;
        }
    }
}
