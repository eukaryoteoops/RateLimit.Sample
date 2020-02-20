using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace RateLimit.Sample
{
    public partial class Startup
    {
        public void AddIpLimit(IServiceCollection services)
        {
            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("RateLimiting"));

            //load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

#if RELEASE
            // inject counter and rules distributed cache stores (IDistributedCache DI required)
            services.AddStackExchangeRedisCache(o =>
            {
                var config = new ConfigurationOptions()
                {
                    AbortOnConnectFail = true,
                    Password = string.Empty,
                    KeepAlive = 60,
                    DefaultDatabase = 0,
                    SyncTimeout = 3000,
                    Ssl = false,
                };
                config.EndPoints.Add(Configuration["Connection:Redis"]);
                o.ConfigurationOptions = config;
            });
            services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
#endif
#if DEBUG
            // inject counter and rules stores
            services.AddMemoryCache();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
#endif
            // configuration (resolvers, counter key builders)
            services.AddHttpContextAccessor();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        public void AddClientLimit(IServiceCollection services)
        {

            //load general configuration from appsettings.json
            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("RateLimiting"));

            //load client rules from appsettings.json
            services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));

#if RELEASE
            // inject counter and rules distributed cache stores (IDistributedCache DI required)
            services.AddStackExchangeRedisCache(o =>
            {
                var config = new ConfigurationOptions()
                {
                    AbortOnConnectFail = true,
                    Password = string.Empty,
                    KeepAlive = 60,
                    DefaultDatabase = 0,
                    SyncTimeout = 3000,
                    Ssl = false,
                };
                config.EndPoints.Add(Configuration["Connection:Redis"]);
                o.ConfigurationOptions = config;
            });
            services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
#endif
#if DEBUG
            // inject counter and rules stores
            services.AddMemoryCache();
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
#endif
            // configuration (resolvers, counter key builders)
            services.AddHttpContextAccessor();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
