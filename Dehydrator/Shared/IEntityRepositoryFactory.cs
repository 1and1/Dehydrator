using System;
using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Provides <see cref="IEntityRepository{T}"/>s for specific <see cref="IEntity"/> types.
    /// </summary>
    public interface IEntityRepositoryFactory
    {
        /// <summary>
        /// Returns a repository for a specific type of <see cref="IEntity"/>.
        /// </summary>
        [NotNull]
        IEntityRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new();

        /// <summary>
        /// Returns a repository for a specific type of <see cref="IEntity"/>.
        /// </summary>
        [NotNull]
        IEntityRepository<IEntity> Create([NotNull] Type entityType);
    }
}
