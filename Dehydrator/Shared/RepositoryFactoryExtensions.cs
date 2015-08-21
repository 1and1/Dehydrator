using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Dehydrator
{
    public static class RepositoryFactoryExtensions
    {
        private static readonly MethodInfo CreateMethod = typeof(IRepositoryFactory).GetMethod(nameof(IRepositoryFactory.Create));

        /// <summary>
        /// Returns a repository for a specific type of <see cref="IEntity"/>.
        /// </summary>
        /// <remarks>A full implementation of <see cref="IRepository{TEntity}"/>. The return type <see cref="IReadRepository{TEntity}"/> is only used for covariance. You can downcast to get write access.</remarks>
        [NotNull]
        public static IReadRepository<IEntity> Create([NotNull] this IRepositoryFactory repositoryFactory, [NotNull] Type entityType)
        {
            return (IReadRepository<IEntity>)CreateMethod.MakeGenericMethod(entityType).Invoke(repositoryFactory, null);
        }
    }
}
