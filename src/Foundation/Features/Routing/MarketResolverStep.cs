using EPiServer.Core.Routing;
using EPiServer.Core.Routing.Internal;
using EPiServer.Core.Routing.Pipeline;
using EPiServer.Core.Routing.Pipeline.Internal;
using System.Globalization;

namespace Foundation.Features.Routing
{
    public class MarketResolverStep : IUrlResolverPipelineStep
    {
        public RoutingState Resolve(UrlResolverContext context, UrlResolverOptions options)
        {
            var httpAccessor = ServiceLocator.Current.GetInstance<IHttpContextAccessor>();

            var request = httpAccessor.HttpContext?.Request;

            var currentLanguage = request?.Cookies["Language"] ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName; //

            var currentMarket = ServiceLocator.Current.GetInstance<ICurrentMarket>();
            var market = currentMarket.GetCurrentMarket()?.MarketId.ToString().ToLower();

            var segment = context.GetNextSegment();

            if (!segment.Next.IsEmpty)
            {
                // Check if the segment matches our market
                if (segment.Next.Span.Equals($"{currentLanguage}-{market}", StringComparison.OrdinalIgnoreCase))
                {
                    // It did, store it so we can use it later
                    context.RouteValues["CurrentMarket"] = $"{currentLanguage}-{market}";

                    // The remaining parts should be handled by the rest of the
                    // steps in the pipeline
                    context.RemainingSegments = segment.Remaining;
                }
            }

            return RoutingState.Continue;
        }
    }
}
