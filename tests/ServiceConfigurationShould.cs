using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.ServiceDiscovery;
using Xunit;

namespace Ocelot.Provider.Static.Tests
{
    public class ServiceConfigurationShould
    {
        [Fact]
        public void RegisterAppConfigurationProviderFactory()
        {
            var serviceCollection = new ServiceCollection();
            var builder = new OcelotBuilder(serviceCollection, new ConfigurationBuilder().Build());

            builder.AddAppConfiguration();

            ServiceDiscoveryFinderDelegate factory = serviceCollection
                .BuildServiceProvider()
                .GetService<ServiceDiscoveryFinderDelegate>();

            factory.Should().NotBeNull();
        }
    }
}
