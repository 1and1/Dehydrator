using System.Data.Entity;
using Dehydrator.EntityFramework;
using Microsoft.Practices.Unity;

namespace Dehydrator.Unity
{
    public static class UnityContainerExtensions
    {
        /// <summary>
        /// Registers database-backed repository factories for a specific <see cref="DbContext"/>.
        /// </summary>
        /// <typeparam name="TDbContext">The specific type of <see cref="DbContext"/> to register for.</typeparam>
        /// <param name="container">The Unity container to perform the registration on.</param>
        /// <param name="dehydrate"><see langword="true"/> to wrap factories in <see cref="DehydratingReadRepositoryFactory"/>s/<see cref="DehydratingCrudRepositoryFactory"/>s.</param>
        /// <param name="readOnly"><see langword="true"/> to register only <see cref="IReadRepositoryFactory"/>, <see langword="false"/> to also add <see cref="ICrudRepositoryFactory"/>.</param>
        /// <returns>A fluent API object to continue the configuration.</returns>
        public static DatabaseRegistration<TDbContext> RegisterDatabase<TDbContext>(this IUnityContainer container,
            bool dehydrate = false, bool readOnly = false)
            where TDbContext : DbContext, new()
        {
            container.RegisterType<TDbContext>();

            container.RegisterType<IReadRepositoryFactory>(
                $"{nameof(IReadRepositoryFactory)} for {typeof(TDbContext).FullName}",
                new InjectionFactory(c =>
                {
                    var dbFactory = new DbReadRepositoryFactory(c.Resolve<TDbContext>());
                    return dehydrate ? new DehydratingReadRepositoryFactory(dbFactory) : (IReadRepositoryFactory)dbFactory;
                }));

            if (!readOnly)
            {
                container.RegisterType<ICrudRepositoryFactory>(
                    $"{nameof(ICrudRepositoryFactory)} for {typeof(TDbContext).FullName}",
                    new InjectionFactory(c =>
                    {
                        var dbFactory = new DbCrudRepositoryFactory(c.Resolve<TDbContext>());
                        return dehydrate ? new DehydratingCrudRepositoryFactory(dbFactory) : (ICrudRepositoryFactory)dbFactory;
                    }));
            }

            return new DatabaseRegistration<TDbContext>(container, dehydrate, readOnly);
        }
    }
}