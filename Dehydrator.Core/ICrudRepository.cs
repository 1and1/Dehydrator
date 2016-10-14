using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Provides CRUD access to a set of <see cref="IEntity"/>s. Usually backed by a database.
    /// </summary>
    public interface ICrudRepository<TEntity> : IReadRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity with <see cref="IEntity.Id"/> set.</returns>
        /// <remarks>Results may be dehydrated.</remarks>
        /// <exception cref="DataException">The underlying storage system failed to persist the changes.</exception>
        [NotNull]
        [DataObjectMethod(DataObjectMethodType.Insert)]
        TEntity Add([NotNull] TEntity entity);

        /// <summary>
        /// Modifies an existing entity in the database.
        /// </summary>
        /// <param name="entity">The modified entity.</param>
        /// <exception cref="KeyNotFoundException">No existing entity with matching <see cref="IEntity.Id"/> in the backing database.</exception>
        /// <exception cref="DataException">The underlying storage system failed to persist the changes.</exception>
        [DataObjectMethod(DataObjectMethodType.Update)]
        void Modify([NotNull] TEntity entity);

        /// <summary>
        /// Removes a specific entity from the database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to remove.</param>
        /// <returns><c>true</c> if the entity was removed; <c>false</c> if the entity did not exist.</returns>
        /// <exception cref="DataException">The underlying storage system failed to persist the changes.</exception>
        [DataObjectMethod(DataObjectMethodType.Delete)]
        bool Remove(long id);

        /// <summary>
        /// Locks the contents represented by the repository. Any following changes are only commited if <see cref="ITransaction.Commit"/> is called.
        /// </summary>
        /// <returns>A representation of the transaction. Dispose to end the transaction and rollback uncomitted changes.</returns>
        [NotNull]
        ITransaction BeginTransaction();

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The added entity with <see cref="IEntity.Id"/> set.</returns>
        /// <remarks>Results may be dehydrated.</remarks>
        /// <exception cref="DataException">The underlying storage system failed to persist the changes.</exception>
        [ItemNotNull]
        Task<TEntity> AddAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Modifies an existing entity in the database.
        /// </summary>
        /// <param name="entity">The modified entity.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="KeyNotFoundException">No existing entity with matching <see cref="IEntity.Id"/> in the backing database.</exception>
        /// <exception cref="DataException">The underlying storage system failed to persist the changes.</exception>
        Task ModifyAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Removes a specific entity from the database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to remove.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns><c>true</c> if the entity was removed; <c>false</c> if the entity did not exist.</returns>
        /// <exception cref="DataException">The underlying storage system failed to persist the changes.</exception>
        Task<bool> RemoveAsync(long id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Locks the contents represented by the repository. Any following changes are only commited if <see cref="ITransaction.Commit"/> is called.
        /// </summary>
        /// <returns>A representation of the transaction. Dispose to end the transaction and rollback uncomitted changes.</returns>
        [ItemNotNull]
        Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
