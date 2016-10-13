using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dehydrator
{
    /// <summary>
    /// An <see cref="IEnumerable{T}"/> object (usually <see cref="IQueryable{T}"/>) that provides asynchronous collectors. Usually accessed via <see cref="QueryableExtensions"/>.
    /// </summary>
    public interface IAsyncCollectable<T> : IEnumerable<T>
    {
        /// <summary>
        /// Requests that the underlying database provider performs appropriate joins to eager-load the specified <paramref name="path"/>.
        /// </summary>
        IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> path);

        /// <summary>
        /// Creates a list from the queryable. Performs the operation asynchronously.
        /// </summary>
        Task<List<T>> ToListAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the first element from the queryable. Performs the operation asynchronously.
        /// </summary>
        Task<T> FirstAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the first element from the queryable or the default value of <typeparamref name="T"/> if there are no elements. Performs the operation asynchronously.
        /// </summary>
        Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}