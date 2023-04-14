using EPiServer.Core.Routing.Pipeline;
using EPiServer.Core.Routing.Pipeline.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Foundation.Features.Routing
{
    public class RegisterRoutingPipelineSteps : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                var generatorPipelineRegistry = builder.ApplicationServices.GetService<UrlGeneratorPipelineRegistry>();
                var resolverPipelineRegistry = builder.ApplicationServices.GetService<UrlResolverPipelineRegistry>();

                var startPageDefaultMode = GetGeneratorDefinition("StartPage", RouteContextMode.Default);
                startPageDefaultMode.Insert(
                    startPageDefaultMode.FindIndex(x => x.Type == typeof(HostUrlGeneratorPipelineStep)),
                    new PipelineStepDefinition
                    {
                        Type = typeof(MarketGeneratorStep),
                    });

                startPageDefaultMode = GetResolverDefinition("StartPage", RouteContextMode.Default);
                startPageDefaultMode.Insert(
                    startPageDefaultMode.FindIndex(x => x.Type == typeof(LanguageUrlResolverPipelineStep)),
                    new PipelineStepDefinition
                    {
                        Type = typeof(MarketResolverStep),
                    });

                var startPageEditMode = GetGeneratorDefinition("StartPage", RouteContextMode.Edit);
                startPageEditMode.Insert(
                    startPageEditMode.FindIndex(x => x.Type == typeof(LanguageUrlGeneratorPipelineStep)),
                    new PipelineStepDefinition
                    {
                        Type = typeof(MarketGeneratorStep),
                    });

                startPageEditMode = GetResolverDefinition("StartPageEdit", RouteContextMode.Edit);
                startPageEditMode.Insert(
                    startPageEditMode.FindIndex(x => x.Type == typeof(LanguageUrlResolverPipelineStep)),
                    new PipelineStepDefinition
                    {
                        Type = typeof(MarketResolverStep),
                    });

                PipelineDefinition GetGeneratorDefinition(string name, RouteContextMode contextMode)
                {
                    return generatorPipelineRegistry.All.FirstOrDefault(x =>
                        x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                        x.ContextMode == contextMode);
                }

                PipelineDefinition GetResolverDefinition(string name, RouteContextMode contextMode)
                {
                    return resolverPipelineRegistry.All.FirstOrDefault(x =>
                        x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                        x.ContextMode == contextMode);
                }

                next(builder);
            };
        }
    }
}
