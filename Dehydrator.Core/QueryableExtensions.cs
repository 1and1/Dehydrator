using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Provides extension methods for <see cref="IQueryable{T}"/>s.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Creates a list from the queryable. Performs the operation asynchronously if possible.
        /// </summary>
        public static Task<List<T>> ToListAsync<T>([NotNull] this IQueryable<T> queryable, CancellationToken cancellationToken = default(CancellationToken)) =>
            (queryable as IAsyncCollectable<T>)?.ToListAsync(cancellationToken) ?? Task.FromResult(queryable.ToList());

        /// <summary>
        /// Returns the first element from the queryable. Performs the operation asynchronously if possible.
        /// </summary>
        public static Task<T> FirstAsync<T>([NotNull] this IQueryable<T> queryable, CancellationToken cancellationToken = default(CancellationToken)) =>
            (queryable as IAsyncCollectable<T>)?.FirstAsync(cancellationToken) ?? Task.FromResult(queryable.First());

        /// <summary>
        /// Returns the first element from the queryable or the default value of <typeparamref name="T"/> if there are no elements. Performs the operation asynchronously if possible.
        /// </summary>
        public static Task<T> FirstOrDefaultAsync<T>([NotNull] this IQueryable<T> queryable, CancellationToken cancellationToken = default(CancellationToken)) =>
            (queryable as IAsyncCollectable<T>)?.FirstOrDefaultAsync(cancellationToken) ?? Task.FromResult(queryable.FirstOrDefault());
    }
}
