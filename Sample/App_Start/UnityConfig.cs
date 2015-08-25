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
                .EnableRepositoryFactory();
        }

        private static IUnityContainer RegisterDehydratedDbRepository(this IUnityContainer container)
        {
            return container.RegisterType<IRepositoryFactory>(new InjectionFactory(c =>
                new DehydratingRepositoryFactory(new DbRepositoryFactory(c.Resolve<DbContext>()))));
        }

        private static IUnityContainer EnableRepositoryFactory(this IUnityContainer container)
        {
            return container.RegisterType(typeof(IRepository<>), new InjectionFactory((c, t, s) =>
                c.Resolve<IRepositoryFactory>().Create(t.GetGenericArguments()[0])));
        }
    }
}