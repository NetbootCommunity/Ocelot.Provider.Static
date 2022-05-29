using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Ocelot.Configuration;
using Ocelot.Logging;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;

namespace Ocelot.Provider.Static
{
    /// <summary>
    /// Provider for getting services from app configuration.
    /// </summary>
    public class StaticConfiguration : IServiceDiscoveryProvider
    {
        private const int DefaultCacheExpirationInMinutes = 10;
        private const string DefaultServiceSectionName = "Services";
        private const string SectionNameConfigKey = "ServiceDiscoveryProvider:AppConfigurationSectionName";

        private readonly IConfiguration _configuration;
        private readonly string _serviceName;
        private readonly ServiceProviderConfiguration _providerConfiguration;
        private readonly IMemoryCache _cache;
        private readonly IOcelotLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="downstreamReRoute">The downstream re route.</param>
        /// <param name="providerConfiguration">The provider configuration.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="factory">The factory.</param>
        public StaticConfiguration(
            IConfiguration configuration,
            DownstreamRoute downstreamReRoute,
            ServiceProviderConfiguration providerConfiguration,
            IMemoryCache cache,
            IOcelotLoggerFactory factory)
        {
            _configuration = configuration;
            _serviceName = downstreamReRoute.ServiceName.ToLower();
            _providerConfiguration = providerConfiguration ?? throw new ArgumentNullException(nameof(providerConfiguration));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = factory.CreateLogger<StaticConfiguration>();
        }

        /// <summary>
        /// Gets services.
        /// </summary>
        public Task<List<Service>> Get()
            => Task.FromResult(GetServices());

        /// <summary>
        /// Gets the ocelot services configurations with caching.
        /// </summary>
        private List<Service> GetServices()
        {
            if (!_cache.TryGetValue(GetCacheKey(), out List<Service> services))
            {
                services = GetServiceConfigurations();
                if (services != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(GetCacheExpiration());
                    _cache.Set(GetCacheKey(), services, cacheEntryOptions);
                }
                else
                {
                    _logger.LogWarning(
                        $"Unable to use service '{_serviceName}' as it is invalid. Service is missing in configuration");
                    return new List<Service>();
                }
            }

            return services;
        }

        /// <summary>
        /// Gets the ocelot service configurations.
        /// </summary>
        /// <returns></returns>
        private List<Service> GetServiceConfigurations()
        {
            var configs = _configuration.GetSection(GetSectionName())
                .Get<Dictionary<string, List<ServiceConfiguration>>>()
                .Where(x => x.Key.Equals(_serviceName, StringComparison.OrdinalIgnoreCase))
                .SelectMany(x => x.Value)
                .ToList();
            if (configs == null)
                return default;

            var services = new List<Service>();
            foreach (var config in configs)
            {
                services.Add(new Service(
                    name: _serviceName,
                    hostAndPort: new ServiceHostAndPort(
                        config.DownstreamPath.Host,
                        config.DownstreamPath.Port,
                        config.DownstreamPath.Scheme),
                    string.Empty,
                    string.Empty,
                    Array.Empty<string>()));
            }
            return services;
        }

        /// <summary>
        /// Gets the cache expiration.
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetCacheExpiration()
            => _providerConfiguration.PollingInterval > 0
            ? TimeSpan.FromMilliseconds(_providerConfiguration.PollingInterval)
            : TimeSpan.FromMinutes(DefaultCacheExpirationInMinutes);

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <returns></returns>
        private string GetCacheKey()
            => $"Service_{_serviceName}".ToLower();

        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        /// <returns></returns>
        private string GetSectionName() =>
            _configuration.GetValue<string>(SectionNameConfigKey) ?? DefaultServiceSectionName;
    }
}