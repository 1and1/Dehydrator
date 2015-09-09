using Dehydrator.Unity;
using Microsoft.Practices.Unity;

namespace Dehydrator.Sample
{
    public static class UnityConfig
    {
        public static IUnityContainer InitContainer()
        {
            var container = new UnityContainer();

            container.RegisterDatabase<SampleDbContext>()
                .RegisterRepositories();

            //container.RegisterDatabase<SampleDbContext>()
            //    .RegisterRepository(x => x.Packages)
            //    .RegisterRepository(x => x.PackagesConfig);

            return container;
        }
    }
}