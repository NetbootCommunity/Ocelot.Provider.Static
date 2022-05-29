using Microsoft.Extensions.Configuration;
using Ocelot.ServiceDiscovery;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Ocelot.Logging;

namespace Ocelot.Provider.Static
{
    /// <summary>
    /// Factory for creating <see cref="StaticConfiguration"/> provider.
    /// </summary>
    public static class StaticConfigurationProviderFactory
    {
        /// <summary>
        /// Get <see cref="StaticConfiguration"/> provider.
        /// </summary>
        public static ServiceDiscoveryFinderDelegate Get = (provider, config, reRoute) =>
        {
            IConfiguration configuration = provider.GetService<IConfiguration>();
            IMemoryCache cache = provider.GetService<IMemoryCache>();
            IOcelotLoggerFactory loggerFactory = provider.GetService<IOcelotLoggerFactory>();

            return new StaticConfiguration(configuration, reRoute, config, cache, loggerFactory);
        };
    }
}
