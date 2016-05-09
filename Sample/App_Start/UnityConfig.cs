using Dehydrator.EntityFramework.Unity;
using Microsoft.Practices.Unity;

namespace DehydratorSample
{
    public static class UnityConfig
    {
        public static IUnityContainer InitContainer()
        {
            var container = new UnityContainer();

            container.RegisterDatabase<SampleDbContext>(dehydrate: true)
                .RegisterRepositories();

            //container.RegisterDatabase<SampleDbContext>(dehydrate: true)
            //    .RegisterRepository(x => x.Packages)
            //    .RegisterRepository(x => x.PackagesConfig);

            return container;
        }
    }
}