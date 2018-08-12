﻿namespace Ocelot.Provider.Eureka.UnitTests
{
    using DependencyInjection;
    using DownstreamRouteFinder.Middleware;
    using DownstreamUrlCreator.Middleware;
    using LoadBalancer.Middleware;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Middleware;
    using Middleware.Pipeline;
    using Pivotal.Discovery.Client;
    using Request.Middleware;
    using Shouldly;
    using Steeltoe.Common.Discovery;
    using Steeltoe.Discovery.Eureka;
    using TestStack.BDDfy;
    using WebSockets.Middleware;
    using Xunit;

    public class OcelotPipelineExtensionsTests
    {
        private OcelotPipelineBuilder _builder;
        private OcelotRequestDelegate _handlers;

        [Fact]
        public void should_set_up_pipeline()
        {
            this.Given(_ => GivenTheDepedenciesAreSetUp())
                 .When(_ => WhenIBuild())
                 .Then(_ => ThenThePipelineIsBuilt())
                 .BDDfy();
        }

        private void ThenThePipelineIsBuilt()
        {
            _handlers.ShouldNotBeNull();
        }

        private void WhenIBuild()
        {
            _handlers = _builder.BuildOcelotPipeline(new OcelotPipelineConfiguration());
        }

        private void WhenIExpandBuild()
        {
            OcelotPipelineConfiguration configuration = new OcelotPipelineConfiguration();
            configuration.MapWhenOcelotPipeline.Add((app) =>
            {
                app.UseDownstreamRouteFinderMiddleware();
                app.UseDownstreamRequestInitialiser();
                app.UseLoadBalancingMiddleware();
                app.UseDownstreamUrlCreatorMiddleware();
                app.UseWebSocketsProxyMiddleware();

                return context => context.HttpContext.WebSockets.IsWebSocketRequest;
            });
            _handlers = _builder.BuildOcelotPipeline(new OcelotPipelineConfiguration());
        }

        private void GivenTheDepedenciesAreSetUp()
        {
            IConfigurationBuilder test = new ConfigurationBuilder();
            var root = test.Build();
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(root);
            services.AddDiscoveryClient(new DiscoveryOptions
            {
                ClientType = DiscoveryClientType.EUREKA,
                ClientOptions = new EurekaClientOptions()
                {
                    ShouldFetchRegistry = false,
                    ShouldRegisterWithEureka = false
                }
            });
            services.AddOcelot();
            var provider = services.BuildServiceProvider();
            _builder = new OcelotPipelineBuilder(provider);
        }
    }
}
