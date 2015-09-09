using System.Data.Entity;
using Dehydrator.EntityFramework;
using Microsoft.Practices.Unity;

namespace Dehydrator.Sample
{
    public static class UnityConfig
    {
        public static IUnityContainer InitContainer()
        {
            return new UnityContainer()
                .RegisterType<DbContext, SampleDbContext>()
                .RegisterDehydratedDbRepository()
                .UseRepositoryFactory();
        }

        private static IUnityContainer RegisterDehydratedDbRepository(this IUnityContainer container)
        {
            return container.RegisterType<ICrudRepositoryFactory>(new InjectionFactory(c =>
                new DehydratingCrudRepositoryFactory(new DbCrudRepositoryFactory(c.Resolve<DbContext>()))));
        }

        private static IUnityContainer UseRepositoryFactory(this IUnityContainer container)
        {
            return container
                .RegisterType(typeof(IReadRepository<>), new InjectionFactory((c, t, s) =>
                    c.Resolve<ICrudRepositoryFactory>().Create(t.GetGenericArguments()[0])))
                .RegisterType(typeof(ICrudRepository<>), new InjectionFactory((c, t, s) =>
                    c.Resolve<ICrudRepositoryFactory>().Create(t.GetGenericArguments()[0])));
        }
    }
}