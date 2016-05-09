using System.Web.Http;
using System.Web.Http.Validation;
using Dehydrator.WebApi;
using Newtonsoft.Json;
using Unity.WebApi;

namespace DehydratorSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes(new InheritanceRouteProvider());
            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.InitContainer());

            // Prevent Web API from rejecting dehydrated entities
            config.Services.Clear(typeof(ModelValidatorProvider));

            config.Formatters.JsonFormatter.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.EnsureInitialized();
        }
    }
}