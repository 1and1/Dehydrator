using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Provides <see cref="IReadRepository{T}"/>s for specific <see cref="IEntity"/> types.
    /// </summary>
    public interface IReadRepositoryFactory
    {
        /// <summary>
        /// Returns a read-only repository for a specific type of <see cref="IEntity"/>.
        /// </summary>
        [NotNull]
        IReadRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new();
    }
}