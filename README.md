# Ocelot Static Discovery provider

This package adds to the ocelot the ability to define static services in your configuration.

> Ocelot allows you to specify a service discovery provider and will use this to find the host and port for the downstream service Ocelot is forwarding a request to.
> At the moment this is only supported in the GlobalConfiguration section which means the same service discovery provider will be used for all Routes you specify a ServiceName for at Route level.

## Please show the value

Choosing a project dependency could be difficult. We need to ensure stability and maintainability of our projects.
Surveys show that GitHub stars count play an important factor when assessing library quality.

⭐ Please give this repository a star. It takes seconds and help thousands of developers! ⭐

## Support development

It doesn't matter if you are a professional developer, creating a startup or work for an established company.
All of us care about our tools and dependencies, about stability and security, about time and money we can safe, about quality we can offer.
Please consider sponsoring to give me an extra motivational push to develop the next great feature.

> If you represent a company, want to help the entire community and show that you care, please consider sponsoring using one of the higher tiers.
Your company logo will be shown here for all developers, building a strong positive relation.

## Installation

The library is available as a nuget package. You can install it as any other nuget package from your IDE, try to search by `Ocelot.Provider.Static`. You can find package details [on this webpage](https://www.nuget.org/packages/Ocelot.Provider.Static).

```xml
// Package Manager
Install-Package Ocelot.Provider.Static

// .NET CLI
dotnet add package Ocelot.Provider.Static

// Package reference in .csproj file
<PackageReference Include="Ocelot.Provider.Static" Version="6.0.0" />
```

Then add the following to your ConfigureServices method.

```csharp
s.AddOcelot()
    ..AddStaticConfiguration();
```

The following is required in the GlobalConfiguration.

```json
"GlobalConfiguration": {
  "ServiceDiscoveryProvider": {
    "Type": "StaticConfiguration",
    "PollingIntervalSeconds":  10000
  }
}
```

In order to tell Ocelot a Route is to use the service discovery provider for its host and port you must add the ServiceName and load balancer you wish to use when making requests downstream.
At the moment Ocelot has a RoundRobin and LeastConnection algorithm you can use.
If no load balancer is specified Ocelot will not load balance requests.

```json
{
    "DownstreamPathTemplate": "/v2/{everything}",
    "DownstreamScheme": "http",
    "UpstreamPathTemplate": "/petstore/{everything}",
    "UpstreamHttpMethod": [ "GET", "POST", "PUT", "DELETE" ],
    "ServiceName": "petstore",
    "LoadBalancerOptions": {
        "Type": "LeastConnection"
    },
}
```

## Configuration

The following example implemented the static configuration provider.

```json
{
  "Services": {
    "Users": [
      {
        "DownstreamPath": "http://localhost:9003"
      }
    ],
    "projects": [
      {
        "DownstreamPath": "http://localhost:9002"
      }
    ],
    "Authorization": [
      {
        "DownstreamPath": "https://authorizationService.domain.com"
      }
    ]
  },
}
```

### Polling

This provider from optimization reason cache services 5 minutes by default
If you want change or turn off this caching, then set `PollingInterval` in provider definition (miliseconds).

### Change services definition section name

If you want change default section name, than you can set property `AppConfigurationSectionName` in provider definition.

```json
"GlobalConfiguration": {
    "ServiceDiscoveryProvider": {
        "Type": "StaticConfiguration",
        "PollingIntervalSeconds":  10000,
        "AppConfigurationSectionName": "ApiServices"
    }
}
```

## How to Contribute

Everyone is welcome to contribute to this project! Feel free to contribute with pull requests, bug reports or enhancement suggestions.

## Bugs and Feedback

For bugs, questions and discussions please use the [GitHub Issues](https://github.com/NetbootCommunity/Ocelot-Provider-Static/issues).

## License

This project is licensed under [MIT License](https://github.com/NetbootCommunity/Ocelot-Provider-Static/blob/main/LICENSE).
