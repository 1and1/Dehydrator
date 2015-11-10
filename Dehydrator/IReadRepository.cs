using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace Dehydrator
{
    /// <summary>
    /// Provides read-only access to a set of <see cref="IEntity"/>s. Usually backed by a database.
    /// </summary>
    public interface IReadRepository<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Returns all entities of a certain type from the backing database.
        /// </summary>
        /// <remarks><see cref="EntityExtensions.Dehydrate{TEntity}"/> may be called on results.</remarks>
        [NotNull]
        [DataObjectMethod(DataObjectMethodType.Select)]
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Returns all entities of a certain type that match a predicate.
        /// </summary>
        /// <remarks><see cref="EntityExtensions.Dehydrate{TEntity}"/> may be called on results.</remarks>
        [NotNull]
        IEnumerable<TEntity> GetAll([NotNull] Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Checks whether an entity with a given <see cref="IEntity.Id"/> exists in the database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to check.</param>
        bool Exists(long id);

        /// <summary>
        /// Returns a specific entity from the backing database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to find.</param>
        /// <returns>The entity or <see langword="null"/> if there was no match.</returns>
        /// <remarks><see cref="EntityExtensions.DehydrateReferences{TEntity}"/> may be called on results.</remarks>
        [CanBeNull]
        TEntity Find(long id);

#if NET45
        /// <summary>
        /// Performs a LINQ query on the backing database.
        /// </summary>
        /// <remarks><see cref="EntityExtensions.DehydrateReferences{TEntity}"/> may be called on results.</remarks>
        Task<IReadOnlyCollection<TResult>> QueryAsync<TResult>([NotNull] Func<IQueryable<TEntity>, IQueryable<TResult>> query);

        /// <summary>
        /// Performs a LINQ query on the backing database and returns the first result.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <paramref name="query"/> returned no results.</exception>
        /// <remarks><see cref="EntityExtensions.DehydrateReferences{TEntity}"/> may be called on results.</remarks>
        Task<TResult> FirstAsync<TResult>([NotNull] Func<IQueryable<TEntity>, IQueryable<TResult>> query);

        /// <summary>
        /// Performs a LINQ query on the backing database and returns the first result or the default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <remarks><see cref="EntityExtensions.DehydrateReferences{TEntity}"/> may be called on results.</remarks>
        Task<TResult> FirstOrDefaultAsync<TResult>([NotNull] Func<IQueryable<TEntity>, IQueryable<TResult>> query);

        /// <summary>
        /// Returns a specific entity from the backing database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to find.</param>
        /// <returns>The entity or <see langword="null"/> if there was no match.</returns>
        /// <remarks><see cref="EntityExtensions.DehydrateReferences{TEntity}"/> may be called on results.</remarks>
        Task<TEntity> FindAsync(long id);
#endif
    }
}
