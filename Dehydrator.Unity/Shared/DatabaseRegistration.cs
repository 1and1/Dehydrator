using System;
using System.Data.Entity;
using Dehydrator.EntityFramework;
using JetBrains.Annotations;
using Microsoft.Practices.Unity;

namespace Dehydrator.Unity
{
    /// <summary>
    /// A fluent API object returned by <see cref="UnityContainerExtensions.RegisterDatabase{TDbContext}"/> to continue the configuration.
    /// </summary>
    [PublicAPI]
    public class DatabaseRegistration<TDbContext>
        where TDbContext : DbContext
    {
        private readonly IUnityContainer _container;
        private readonly bool _readOnly;

        internal DatabaseRegistration(IUnityContainer container, bool readOnly)
        {
            _container = container;
            _readOnly = readOnly;
        }

        /// <summary>
        /// Registers dehydrating database-backed <see cref="IReadRepositoryFactory"/>s and <see cref="ICrudRepositoryFactory"/>s (if not read-only) for any <see cref="IEntity"/> type.
        /// </summary>
        public void RegisterRepositories()
        {
            _container.RegisterType(typeof(IReadRepository<>), new InjectionFactory((c, t, s) =>
            {
                var factory = c.Resolve<IReadRepositoryFactory>(
                    name: $"{nameof(IReadRepositoryFactory)} for {typeof(TDbContext).FullName}");
                var entityType = t.GetGenericArguments()[0];
                return factory.Create(entityType);
            }));

            if (!_readOnly)
            {
                _container.RegisterType(typeof(ICrudRepository<>), new InjectionFactory((c, t, s) =>
                {
                    var factory = c.Resolve<ICrudRepositoryFactory>(
                        name: $"{nameof(ICrudRepositoryFactory)} for {typeof(TDbContext).FullName}");
                    var entityType = t.GetGenericArguments()[0];
                    return factory.Create(entityType);
                }));
            }
        }

        /// <summary>
        /// Registers a dehydrating database-backed <see cref="IReadRepositoryFactory"/> and <see cref="ICrudRepositoryFactory"/> (if not read-only) for a specific <see cref="DbSet{TEntity}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The specific <see cref="IEntity"/> type of the <see cref="DbSet{TEntity}"/>.</typeparam>
        /// <param name="setSelector">Used to select a <see cref="DbSet{TEntity}"/> contained in a <see cref="DbContext"/>.</param>
        /// <returns>A fluent API object to continue the configuration.</returns>
        public DatabaseRegistration<TDbContext> RegisterRepository<TEntity>(
            Func<TDbContext, DbSet<TEntity>> setSelector)
            where TEntity : class, IEntity, new()
        {
            _container.RegisterType<IReadRepository<TEntity>>(new InjectionFactory((c, t, s) =>
            {
                var dbContext = c.Resolve<TDbContext>();
                var dbRepo = new DbReadRepository<TEntity>(setSelector(dbContext));
                return new DehydratingReadRepository<TEntity>(dbRepo);
            }));

            if (!_readOnly)
            {
                _container.RegisterType<ICrudRepository<TEntity>>(new InjectionFactory((c, t, s) =>
                {
                    var factory = c.Resolve<ICrudRepositoryFactory>(
                        name: $"{nameof(ICrudRepositoryFactory)} for {typeof(TDbContext).FullName}");
                    return new DehydratingCrudRepository<TEntity>(factory);
                }));
            }

            return this;
        }
    }
}