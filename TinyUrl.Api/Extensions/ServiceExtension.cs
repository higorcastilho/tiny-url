using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyUrl.Data.Interface;
using TinyUrl.Data.Repository;
using TinyUrl.Service.Interface;
using TinyUrl.Service.Services;

namespace TinyUrl.Api.Extensions
{
    public static class ServiceExtension
    {
        public static void AddInjections(this IServiceCollection services)
        {
            services.AddHostedService<GetRangeHostedService>();

            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ILinkService, LinkService>();
        }
    }
}
