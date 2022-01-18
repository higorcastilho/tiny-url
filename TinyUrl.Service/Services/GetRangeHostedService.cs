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
using TinyUrl.Service.Interface;

namespace TinyUrl.Service.Services
{
    public class GetRangeHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public GetRangeHostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                cacheService.GetRange();
                
                return Task.CompletedTask;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
