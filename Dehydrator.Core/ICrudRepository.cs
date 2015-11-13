using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace Dehydrator
{
    /// <summary>
    /// Provides CRUD access to a set of <see cref="IEntity"/>s. Usually backed by a database.
    /// </summary>
    public interface ICrudRepository<TEntity> : IReadRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// Adds a new entity to the database. Call <see cref="SaveChanges"/> when done.
        /// </summary>
        /// <returns>The added entity with <see cref="IEntity.Id"/> set.</returns>
        [NotNull]
        [DataObjectMethod(DataObjectMethodType.Insert)]
        TEntity Add([NotNull] TEntity entity);

        /// <summary>
        /// Modifies an existing entity in the database. Call <see cref="SaveChanges"/> when done.
        /// </summary>
        /// <param name="entity">The modified entity.</param>
        /// <exception cref="KeyNotFoundException">No existing entity with matching <see cref="IEntity.Id"/> in the backing database.</exception>
        [DataObjectMethod(DataObjectMethodType.Update)]
        void Modify([NotNull] TEntity entity);

        /// <summary>
        /// Removes a specific entity from the database. Call <see cref="SaveChanges"/> when done.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to remove.</param>
        /// <returns><see langword="true"/> if the entity was removed; <see langword="false"/> if the entity did not exist.</returns>
        [DataObjectMethod(DataObjectMethodType.Delete)]
        bool Remove(long id);

        /// <summary>
        /// Locks the contents represented by the repository. Any following changes are only commited if <see cref="ITransaction.Commit"/> is called.
        /// </summary>
        /// <returns>A representation of the transaction. Dispose to end the transaction and rollback uncomitted changes.</returns>
        [NotNull]
        ITransaction BeginTransaction();

        /// <summary>
        /// Persists any changes made to the underlying storage system.
        /// </summary>
        /// <exception cref="DataException">The underlying storage system failed to persist the changes.</exception>
        void SaveChanges();

#if NET45
        /// <summary>
        /// Modifies an existing entity in the database. Call <see cref="SaveChangesAsync"/> when done.
        /// </summary>
        /// <param name="entity">The modified entity.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="KeyNotFoundException">No existing entity with matching <see cref="IEntity.Id"/> in the backing database.</exception>
        Task ModifyAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Removes a specific entity from the database. Call <see cref="SaveChangesAsync"/> when done.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to remove.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns><see langword="true"/> if the entity was removed; <see langword="false"/> if the entity did not exist.</returns>
        Task<bool> RemoveAsync(long id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Locks the contents represented by the repository. Any following changes are only commited if <see cref="ITransaction.Commit"/> is called.
        /// </summary>
        /// <returns>A representation of the transaction. Dispose to end the transaction and rollback uncomitted changes.</returns>
        Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Persists any changes made to the underlying storage system.
        /// </summary>
        /// <exception cref="DataException">The underlying storage system failed to persist the changes.</exception>
        Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
#endif
    }
}
