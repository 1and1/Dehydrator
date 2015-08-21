using System;
using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Provides <see cref="IRepository{T}"/>s for specific <see cref="IEntity"/> types.
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Returns a repository for a specific type of <see cref="IEntity"/>.
        /// </summary>
        [NotNull]
        IRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new();

        /// <summary>
        /// Returns a repository for a specific type of <see cref="IEntity"/>.
        /// </summary>
        [NotNull]
        IRepository<IEntity> Create([NotNull] Type entityType);
    }
}
