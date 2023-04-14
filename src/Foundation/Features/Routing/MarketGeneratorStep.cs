using EPiServer.Core.Routing;
using EPiServer.Core.Routing.Internal;
using EPiServer.Core.Routing.Pipeline;
using EPiServer.Core.Routing.Pipeline.Internal;
using System.Globalization;

namespace Foundation.Features.Routing
{
    public class MarketGeneratorStep : IUrlGeneratorPipelineStep
    {
        public RoutingState Generate(UrlGeneratorContext context, UrlGeneratorOptions options)
        {
            var httpAccessor = ServiceLocator.Current.GetInstance<IHttpContextAccessor>();

            var request = httpAccessor.HttpContext?.Request;

            var currentLanguage = request?.Cookies["Language"] ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;  
            //var language = currentLanguage.TwoLetterISOLanguageName;

            var currentMarket = ServiceLocator.Current.GetInstance<ICurrentMarket>();
            var market = currentMarket.GetCurrentMarket().MarketId.ToString().ToLower();

            // This adds the segment
            context.PrependSegment($"{currentLanguage}-{market}");

            // Tells the pipeline to continue generating next segment in the URL
            return RoutingState.Continue;
        }
    }
}
