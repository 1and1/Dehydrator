using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

#if NET45
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Dehydrator
{
    /// <summary>
    /// Wraps an <see cref="IQueryable"/> and runs <see cref="EntityExtensions.DehydrateReferences{T}"/> on the results.
    /// </summary>
    /// <seealso cref="DehydratingQueryProvider"/>
    internal class DehydratingQueryable : IQueryable
    {
        private readonly IQueryable _inner;

        public DehydratingQueryable([NotNull] IQueryable inner)
        {
            _inner = inner;
            Provider = new DehydratingQueryProvider(_inner.Provider);
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
    /// Wraps an <see cref="IAsyncQueryable{T}"/> and runs <see cref="EntityExtensions.DehydrateReferences{T}"/> on the results.
    /// </summary>
    /// <seealso cref="DehydratingQueryProvider"/>
    internal class DehydratingQueryable<T> : DehydratingQueryable, IAsyncQueryable<T>
    {
        private readonly IQueryable<T> _inner;

        public DehydratingQueryable([NotNull] IQueryable<T> inner) : base(inner)
        {
            _inner = inner;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var enumerator = _inner.GetEnumerator();
            while (enumerator.MoveNext())
                yield return enumerator.Current.DehydrateReferences();
        }

#if NET45
        public async Task<List<T>> ToListAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await _inner.ToListAsync(cancellationToken)).Select(x => x.DehydrateReferences()).ToList();
        }

        public async Task<T> FirstAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await _inner.FirstAsync(cancellationToken)).DehydrateReferences();
        }

        public async Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _inner.FirstOrDefaultAsync(cancellationToken);

            // ReSharper disable once ExpressionIsAlwaysNull
            return (result == null) ? result : result.DehydrateReferences();
        }
#endif
    }
}