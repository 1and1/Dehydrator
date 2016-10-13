using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Wraps an <see cref="IQueryable"/> and runs <see cref="DehydrationUtils.DehydrateReferences{T}"/> on the results.
    /// </summary>
    /// <remarks>
    /// This implements <see cref="IOrderedQueryable"/> and not just <see cref="IQueryable"/> because some methods like <see cref="Queryable.OrderBy{TSource,TKey}(IQueryable{TSource},Expression{Func{TSource,TKey}})"/> perform downcasts.
    /// It does not imply that all instances of this class are actually ordered. Rely on the type of the reference not the type of the object to determine that.
    /// </remarks>
    /// <seealso cref="DehydratingQueryProvider"/>
    internal class DehydratingQueryable : IOrderedQueryable
    {
        private readonly IQueryable _inner;

        public DehydratingQueryable([NotNull] IQueryable inner)
        {
            _inner = inner;
            Provider = new DehydratingQueryProvider(_inner.Provider);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new InvalidOperationException("Dehydating queryables cannot be enumerated using non-generic IEnumerable interface. Please use IEnumerable<T> instead.");
        }

        public Expression Expression => _inner.Expression;

        public Type ElementType => _inner.ElementType;

        public IQueryProvider Provider { get; }
    }

    /// <summary>
    /// Wraps an <see cref="IAsyncCollectable{T}"/> and runs <see cref="DehydrationUtils.DehydrateReferences{T}"/> on the results.
    /// </summary>
    /// <remarks>
    /// This implements <see cref="IOrderedQueryable{T}"/> and not just <see cref="IQueryable{T}"/> because some methods like <see cref="Queryable.OrderBy{TSource,TKey}(IQueryable{TSource},Expression{Func{TSource,TKey}})"/> perform downcasts.
    /// It does not imply that all instances of this class are actually ordered. Rely on the type of the reference not the type of the object to determine that.
    /// </remarks>
    /// <seealso cref="DehydratingQueryProvider"/>
    internal class DehydratingQueryable<T> : DehydratingQueryable, IOrderedQueryable<T>, IAsyncCollectable<T>
    {
        private readonly IQueryable<T> _inner;

        public DehydratingQueryable([NotNull] IQueryable<T> inner) : base(inner)
        {
            _inner = inner;
        }

        public IEnumerator<T> GetEnumerator()
        {
            using (var enumerator = _inner.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current.DehydrateReferences();
            }
        }

        public IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> path) =>
            new DehydratingQueryable<T>(_inner.Include(path));

        public async Task<List<T>> ToListAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            (await _inner.ToListAsync(cancellationToken)).Select(x => x.DehydrateReferences()).ToList();

        public async Task<T> FirstAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            (await _inner.FirstAsync(cancellationToken)).DehydrateReferences();

        public async Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _inner.FirstOrDefaultAsync(cancellationToken);

            // ReSharper disable once ExpressionIsAlwaysNull
            return (result == null) ? result : result.DehydrateReferences();
        }
    }
}