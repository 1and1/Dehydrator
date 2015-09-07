using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Provides <see cref="ICrudRepository{T}"/>s for specific <see cref="IEntity"/> types.
    /// </summary>
    public interface ICrudRepositoryFactory : IReadRepositoryFactory
    {
        /// <summary>
        /// Returns a CRUD repository for a specific type of <see cref="IEntity"/>.
        /// </summary>
        [NotNull]
        new ICrudRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new();
    }
}
