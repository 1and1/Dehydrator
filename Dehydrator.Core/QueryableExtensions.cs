using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// Requests that the underlying database provider performs appropriate joins to eager-load the specified <paramref name="path"/>.
        /// </summary>
        public static IQueryable<T> Include<T, TProperty>(this IQueryable<T> queryable, Expression<Func<T, TProperty>> path)
        {
            var asyncCollectable = queryable as IAsyncCollectable<T>;
            if (asyncCollectable == null) return queryable;
            else return asyncCollectable.Include(path);
        }

        /// <summary>
        /// Creates a list from the queryable. Performs the operation asynchronously if possible.
        /// </summary>
        public static async Task<List<T>> ToListAsync<T>([NotNull] this IQueryable<T> queryable, CancellationToken cancellationToken = default(CancellationToken))
        {
            var asyncCollectable = queryable as IAsyncCollectable<T>;
            if (asyncCollectable == null) return queryable.ToList();
            else return await asyncCollectable.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Returns the first element from the queryable. Performs the operation asynchronously if possible.
        /// </summary>
        public static async Task<T> FirstAsync<T>([NotNull] this IQueryable<T> queryable, CancellationToken cancellationToken = default(CancellationToken))
        {
            var asyncCollectable = queryable as IAsyncCollectable<T>;
            if (asyncCollectable == null) return queryable.First();
            else return await asyncCollectable.FirstAsync(cancellationToken);
        }

        /// <summary>
        /// Returns the first element from the queryable or the default value of <typeparamref name="T"/> if there are no elements. Performs the operation asynchronously if possible.
        /// </summary>
        public static async Task<T> FirstOrDefaultAsync<T>([NotNull] this IQueryable<T> queryable, CancellationToken cancellationToken = default(CancellationToken))
        {
            var asyncCollectable = queryable as IAsyncCollectable<T>;
            if (asyncCollectable == null) return queryable.FirstOrDefault();
            else return await asyncCollectable.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
