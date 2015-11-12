﻿using System.Collections.Generic;
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
        public static async Task<List<T>> ToListAsync<T>([NotNull] this IQueryable<T> queryable, CancellationToken cancellationToken = default(CancellationToken))
        {
            var asyncQueryable = queryable as IAsyncQueryable<T>;
            if (asyncQueryable == null) return queryable.ToList();
            else return await asyncQueryable.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Returns the first element from the queryable. Performs the operation asynchronously if possible.
        /// </summary>
        public static async Task<T> FirstAsync<T>([NotNull] this IQueryable<T> queryable, CancellationToken cancellationToken = default(CancellationToken))
        {
            var asyncQueryable = queryable as IAsyncQueryable<T>;
            if (asyncQueryable == null) return queryable.First();
            else return await asyncQueryable.FirstAsync(cancellationToken);
        }

        /// <summary>
        /// Returns the first element from the queryable or the default value of <typeparamref name="T"/> if there are no elements. Performs the operation asynchronously if possible.
        /// </summary>
        public static async Task<T> FirstOrDefaultAsync<T>([NotNull] this IQueryable<T> queryable, CancellationToken cancellationToken = default(CancellationToken))
        {
            var asyncQueryable = queryable as IAsyncQueryable<T>;
            if (asyncQueryable == null) return queryable.FirstOrDefault();
            else return await asyncQueryable.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
