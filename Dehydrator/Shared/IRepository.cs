using System.Collections.Generic;
using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace Dehydrator
{
    /// <summary>
    /// Provides CRUD access to a set of <see cref="IEntity"/>s. Usually backed by a database.
    /// </summary>
    public interface IRepository<TEntity> : IReadRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <returns>The added entity with <see cref="IEntity.Id"/> set.</returns>
        [NotNull]
        TEntity Add([NotNull] TEntity entity);

        /// <summary>
        /// Modifies an existing entity in the database.
        /// </summary>
        /// <param name="entity">The modified entity.</param>
        /// <exception cref="KeyNotFoundException">No existing entity with matching <see cref="IEntity.Id"/> in the backing database.</exception>
        void Modify([NotNull] TEntity entity);

        /// <summary>
        /// Removes a specific entity from the database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to remove.</param>
        /// <returns><see langword="true"/> if the entity was removed; <see langword="false"/> if the entity did not exist.</returns>
        bool Remove(long id);

#if NET45
        /// <summary>
        /// Returns a specific entity from the backing database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to find.</param>
        /// <returns>The entity or <see langword="null"/> if there was no match.</returns>
        Task<TEntity> FindAsync(long id);

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <returns>The added entity with <see cref="IEntity.Id"/> set.</returns>
        Task<TEntity> AddAsync([NotNull] TEntity entity);

        /// <summary>
        /// Modifies an existing entity in the database.
        /// </summary>
        /// <param name="entity">The modified entity.</param>
        /// <exception cref="KeyNotFoundException">No existing entity with matching <see cref="IEntity.Id"/> in the backing database.</exception>
        Task ModifyAsync([NotNull] TEntity entity);

        /// <summary>
        /// Removes a specific entity from the database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to remove.</param>
        /// <returns><see langword="true"/> if the entity was removed; <see langword="false"/> if the entity did not exist.</returns>
        Task<bool> RemoveAsync(long id);
#endif
    }
}
