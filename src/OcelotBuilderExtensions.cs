using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;

namespace Ocelot.Provider.Static
{
    /// <summary>
    /// <see cref="IOcelotBuilder"/> extensions.
    /// </summary>
    public static class OcelotBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="StaticConfiguration"/> provider.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static IOcelotBuilder AddAppConfiguration(this IOcelotBuilder builder)
        {
            builder.Services.AddSingleton(StaticConfigurationProviderFactory.Get);
            return builder;
        }
    }
}
