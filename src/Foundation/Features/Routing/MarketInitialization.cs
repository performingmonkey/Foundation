using EPiServer.Core.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using System.Globalization;

namespace Foundation.Infrastructure.Pipeline
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class MarketInitialization : IInitializableModule
    {
        private IContentUrlGeneratorEvents _contentUrlGeneratorEvents = null;
        private IContentUrlResolverEvents _contentUrlResolverEvents = null;
        private ICurrentMarket _currentMarket = null;
        private IHttpContextAccessor _httpContextAccessor = null;

        public void Initialize(InitializationEngine context)
        {
            _contentUrlGeneratorEvents = context.Locate.Advanced.GetInstance<IContentUrlGeneratorEvents>();
            _contentUrlResolverEvents = context.Locate.Advanced.GetInstance<IContentUrlResolverEvents>();
            _currentMarket = context.Locate.Advanced.GetInstance<ICurrentMarket>();
            _httpContextAccessor = context.Locate.Advanced.GetInstance<IHttpContextAccessor>();
            _contentUrlGeneratorEvents.GeneratedUrl += Events_OnGeneratedUrl;
            _contentUrlResolverEvents.ResolvingUrl += Events_OnResolvingUrl;
        }
        public void Uninitialize(InitializationEngine context)
        {
            _contentUrlGeneratorEvents.GeneratedUrl -= Events_OnGeneratedUrl;
            _contentUrlResolverEvents.ResolvingUrl -= Events_OnResolvingUrl;
        }

        private void Events_OnGeneratedUrl(object sender, UrlGeneratorEventArgs e)
        {
            var request = _httpContextAccessor.HttpContext;

            var language = request?.Request?.Cookies["Language"] ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName; // ?? 
            var market =  _currentMarket.GetCurrentMarket()?.MarketId.ToString().ToLower(); //request.Request.Cookies["MarketId"]?.ToLower(); 

            var url = e.Context.Url;

            if (string.IsNullOrEmpty(language) || string.IsNullOrEmpty(market))
            {
                return;
            }

            url.Path = url.Path.Replace($"/{language}-{market}/{language}", $"/{language}-{market}");

            e.Context.Url = url;
        }

        private void Events_OnResolvingUrl(object sender, UrlResolverEventArgs e)
        {
            var request = _httpContextAccessor.HttpContext;

            var language = request?.Request?.Cookies["Language"] ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName; //  ?? 
            var market = _currentMarket.GetCurrentMarket()?.MarketId.ToString().ToLower(); //request.Request.Cookies["MarketId"]?.ToLower();  

            if (string.IsNullOrEmpty(language) || string.IsNullOrEmpty(market))
            {
                return;
            }

            var url = e.Context.Url.PathAndQuery;

            url = url.Replace($"/{language}-{market}/", $"/{language}/");

            e.Context.Url = url;
        }
    }
}
