using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

#if NET45
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Dehydrator.EntityFramework
{
    /// <summary>
    /// Wraps a <see cref="DbSet"/> or an <see cref="IQueryable"/> derived from it.
    /// </summary>
    /// <remarks>
    /// This implements <see cref="IOrderedQueryable"/> and not just <see cref="IQueryable"/> because some methods like <see cref="Queryable.OrderBy{TSource,TKey}(IQueryable{TSource},Expression{Func{TSource,TKey}})"/> perform downcasts.
    /// It does not imply that all instances of this class are actually ordered. Rely on the type of the reference not the type of the object to determine that.
    /// </remarks>
    /// <seealso cref="DbQueryProvider"/>
    internal class DbQueryable : IOrderedQueryable
    {
        private readonly IQueryable _inner;

        public DbQueryable([NotNull] IQueryable dbSet)
        {
            _inner = dbSet;
            Provider = new DbQueryProvider(_inner.Provider);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        public Expression Expression => _inner.Expression;

        public Type ElementType => _inner.ElementType;

        public IQueryProvider Provider { get; }
    }

    /// <summary>
    /// Wraps a <see cref="DbSet{TEntity}"/> or an <see cref="IQueryable{T}"/> derived from it and exposes its asynchronous collectors via <see cref="IAsyncCollectable{T}"/>.
    /// </summary>
    /// <remarks>
    /// This implements <see cref="IOrderedQueryable{T}"/> and not just <see cref="IQueryable{T}"/> because some methods like <see cref="Queryable.OrderBy{TSource,TKey}(IQueryable{TSource},Expression{Func{TSource,TKey}})"/> perform downcasts.
    /// It does not imply that all instances of this class are actually ordered. Rely on the type of the reference not the type of the object to determine that.
    /// </remarks>
    /// <seealso cref="DbQueryProvider"/>
    internal class DbQueryable<T> : DbQueryable, IOrderedQueryable<T>, IAsyncCollectable<T>
    {
        private readonly IQueryable<T> _inner;

        public DbQueryable([NotNull] IQueryable<T> dbSet) : base(dbSet)
        {
            _inner = dbSet;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

#if NET45
        public Task<List<T>> ToListAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return System.Data.Entity.QueryableExtensions.ToListAsync(_inner, cancellationToken);
        }

        public Task<T> FirstAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return System.Data.Entity.QueryableExtensions.FirstAsync(_inner, cancellationToken);
        }

        public Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return System.Data.Entity.QueryableExtensions.FirstOrDefaultAsync(_inner, cancellationToken);
        }
#endif
    }
}