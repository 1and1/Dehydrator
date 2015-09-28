using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Dehydrator
{
    public static class RepositoryFactoryExtensions
    {
        private static readonly MethodInfo CreateReadMethod =
            typeof(IReadRepositoryFactory).GetMethod(nameof(IReadRepositoryFactory.Create));

        /// <summary>
        /// Returns a read-only repository for a specific type of <see cref="IEntity"/>.
        /// </summary>
        [NotNull]
        public static IReadRepository<IEntity> Create([NotNull] this IReadRepositoryFactory repositoryFactory,
            [NotNull] Type entityType)
        {
            return
                (IReadRepository<IEntity>)CreateReadMethod.MakeGenericMethod(entityType).Invoke(repositoryFactory, null);
        }

        private static readonly MethodInfo CreateCrudMethod =
            typeof(ICrudRepositoryFactory).GetMethod(nameof(ICrudRepositoryFactory.Create));

        /// <summary>
        /// Returns a CRUD repository for a specific type of <see cref="IEntity"/>.
        /// </summary>
        /// <remarks>A full implementation of <see cref="ICrudRepository{TEntity}"/>. The return type <see cref="IReadRepository{TEntity}"/> is only used for covariance. You can downcast to get write access.</remarks>
        [NotNull]
        public static IReadRepository<IEntity> Create([NotNull] this ICrudRepositoryFactory repositoryFactory,
            [NotNull] Type entityType)
        {
            return
                (IReadRepository<IEntity>)CreateCrudMethod.MakeGenericMethod(entityType).Invoke(repositoryFactory, null);
        }
    }
}