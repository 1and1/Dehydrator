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
    public interface IEntityRepository<TEntity>
        where TEntity : IEntity
    {
        /// <summary>
        /// Returns all entities of a certain type from the backing database.
        /// </summary>
        [NotNull]
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Checks whether an entity with a given <see cref="IEntity.Id"/> exists in the database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to check.</param>
        bool Exists(int id);

        /// <summary>
        /// Returns a specific entity from the backing database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to find.</param>
        /// <returns>The entity or <see langword="null"/> if there was no match.</returns>
        [CanBeNull]
        TEntity Find(int id);

        /// <summary>
        /// Modifies an existing entity in the database.
        /// </summary>
        /// <param name="entity">The modified entity.</param>
        void Modify([NotNull] TEntity entity);

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <returns>The added entity with <see cref="IEntity.Id"/> set.</returns>
        [NotNull]
        TEntity Add([NotNull] TEntity entity);

        /// <summary>
        /// Removes a specific entity from the database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to remove.</param>
        /// <returns><see langword="true"/> if the entity was removed; <see langword="false"/> if the entity did not exist.</returns>
        bool Remove(int id);

#if NET45
        /// <summary>
        /// Returns a specific entity from the backing database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to find.</param>
        /// <returns>The entity or <see langword="null"/> if there was no match.</returns>
        Task<TEntity> FindAsync(int id);

        /// <summary>
        /// Modifies an existing entity in the database.
        /// </summary>
        /// <param name="entity">The modified entity.</param>
        Task ModifyAsync([NotNull] TEntity entity);

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <returns>The added entity with <see cref="IEntity.Id"/> set.</returns>
        Task<TEntity> AddAsync([NotNull] TEntity entity);

        /// <summary>
        /// Removes a specific entity from the database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to remove.</param>
        /// <returns><see langword="true"/> if the entity was removed; <see langword="false"/> if the entity did not exist.</returns>
        Task<bool> RemoveAsync(int id);
#endif
    }
}
