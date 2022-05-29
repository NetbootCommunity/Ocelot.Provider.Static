using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration;
using Ocelot.Configuration.Builder;
using Ocelot.DependencyInjection;
using Ocelot.ServiceDiscovery.Providers;
using Xunit;

namespace Ocelot.Provider.Static.Tests
{
    public class StaticConfigurationProviderFactoryShould
    {
        [Fact]
        public void CreateProvider()
        {
            var services = new ServiceCollection();
            var builder = new OcelotBuilder(services, new ConfigurationBuilder().Build());
            builder.Services.AddSingleton(new ConfigurationBuilder().Build());
            builder.Services.AddMemoryCache();

            var factory = StaticConfigurationProviderFactory.Get;

            IServiceDiscoveryProvider provider = factory(
                services.BuildServiceProvider(),
                new ServiceProviderConfiguration("", "", "", 1, "", "", 1),
                new DownstreamRouteBuilder().WithServiceName("Users").Build());

            provider.Should().NotBeNull();
        }
    }
}
