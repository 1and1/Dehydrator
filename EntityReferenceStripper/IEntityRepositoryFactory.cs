using System;
using JetBrains.Annotations;

namespace EntityReferenceStripper
{
    /// <summary>
    /// Provides <see cref="IEntityRepository{T}"/>s for specific <see cref="IEntity"/> types.
    /// </summary>
    public interface IEntityRepositoryFactory
    {
        /// <summary>
        /// Creates a new repository for a specific type of <see cref="IEntity"/>.
        /// </summary>
        [NotNull]
        IEntityRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new();

        /// <summary>
        /// Creates a new repository for a specific type of <see cref="IEntity"/>.
        /// </summary>
        [NotNull]
        IEntityRepository<IEntity> Create([NotNull] Type entityType);
    }
}