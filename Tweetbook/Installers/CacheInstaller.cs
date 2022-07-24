using StackExchange.Redis;
using Tweetbook.Options;
using Tweetbook.Services;

namespace Tweetbook.Installers
{
    public class CacheInstaller : IInstaller
    {
        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {
            var redisCacheSettings = new RedisCacheSettingsOptions();
            configuration.GetSection(nameof(RedisCacheSettingsOptions)).Bind(redisCacheSettings);
            services.AddSingleton(redisCacheSettings);

            if (!redisCacheSettings.Enabled)
            {
                return;
            }

            services.AddStackExchangeRedisCache(opt =>
                {
                    opt.InstanceName = redisCacheSettings.InstanceName;
                    opt.ConfigurationOptions = new ConfigurationOptions
                    {
                        EndPoints = { { redisCacheSettings.Host, redisCacheSettings.Port } },
                        Password = redisCacheSettings.Password,
                        ConnectRetry = redisCacheSettings.ConnectRetry,
                        Ssl = redisCacheSettings.Ssl,
                        AbortOnConnectFail = redisCacheSettings.AbortOnConnectFail,
                        ConnectTimeout = redisCacheSettings.ConnectTimeout,
                        SyncTimeout = redisCacheSettings.SyncTimeout
                    };
                }
            );
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
        }
    }
}
