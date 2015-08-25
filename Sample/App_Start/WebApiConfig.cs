using System.Web.Http;
using Dehydrator.WebApi;
using Newtonsoft.Json;
using Unity.WebApi;

namespace Dehydrator.Sample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.MapHttpAttributeRoutes(new InheritanceRouteProvider());

            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.InitContainer());
        }
    }
}