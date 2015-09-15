using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace Dehydrator
{
    /// <summary>
    /// Provides read-only access to a set of <see cref="IEntity"/>s. Usually backed by a database.
    /// </summary>
    public interface IReadRepository<out TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Returns all entities of a certain type from the backing database.
        /// </summary>
        [NotNull]
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Performs a LINQ query on the backing database.
        /// </summary>
        TResult Query<TResult>([NotNull] Func<IQueryable<TEntity>, TResult> query);

        /// <summary>
        /// Performs a LINQ query on the backing database.
        /// </summary>
        IEnumerable<TResult> Query<TResult>([NotNull] Func<IQueryable<TEntity>, IQueryable<TResult>> query);

        /// <summary>
        /// Performs a LINQ query on the backing database.
        /// </summary>
        IEnumerable<TResult> Query<TResult>([NotNull] Func<IQueryable<TEntity>, IOrderedQueryable<TResult>> query);

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
        [CanBeNull]
        TEntity Find(long id);

#if NET45
        /// <summary>
        /// Performs a LINQ query on the backing database and returns the first result.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <paramref name="query"/> returned no results.</exception>
        Task<TResult> QueryFirstAsync<TResult>([NotNull] Func<IQueryable<TEntity>, IQueryable<TResult>> query);

        /// <summary>
        /// Performs a LINQ query on the backing database and returns the first result or the default value of <typeparamref name="TResult"/>.
        /// </summary>
        Task<TResult> QueryFirstOrDefaultAsync<TResult>([NotNull] Func<IQueryable<TEntity>, IQueryable<TResult>> query);

        /// <summary>
        /// Returns a specific entity from the backing database.
        /// This method is untyped due to limitations of .NET's covariance support. Use <seealso cref="RepositoryExtensions.FindAsync{TEntity}"/> as a wrapper.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to find.</param>
        /// <returns>The entity or <see langword="null"/> if there was no match.</returns>
        Task<IEntity> FindUntypedAsync(long id);
#endif
    }
}
