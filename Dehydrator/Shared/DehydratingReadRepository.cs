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
    /// Decorator for <see cref="IReadRepository{TEntity}"/> instances that transparently dehydrates references on entities it returns.
    /// </summary>
    public class DehydratingReadRepository<TEntity> : IReadRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        [NotNull]
        private readonly IReadRepository<TEntity> _inner;

        /// <summary>
        /// Creates a new reference-dehydrating decorator.
        /// </summary>
        /// <param name="inner">The inner repository to use for the actual storage.</param>
        public DehydratingReadRepository([NotNull] IReadRepository<TEntity> inner)
        {
            _inner = inner;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _inner.GetAll().Select(x => x.DehydrateReferences());
        }

        public TResult Query<TResult>(Func<IQueryable<TEntity>, TResult> query)
        {
            var result = _inner.Query(query);

            // Dehydrate if the query result is an entity, otherwise pass through
            return (typeof(TEntity) == typeof(TResult))
                ? (TResult)(object)((TEntity)(object)result)?.DehydrateReferences()
                : result;
        }

        public IEnumerable<TResult> Query<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query)
        {
            var result = _inner.Query(query);

            // Dehydrate if the query result is a collection of entities, otherwise pass through
            return (typeof(TEntity) == typeof(TResult))
                ? result.Cast<TEntity>().Select(x => x.DehydrateReferences()).Cast<TResult>()
                : result;
        }

        public TEntity Find(long id)
        {
            return _inner.Find(id)?.DehydrateReferences();
        }

        public bool Exists(long id)
        {
            return _inner.Exists(id);
        }

#if NET45
        public async Task<TResult> Query<TResult>(Func<IQueryable<TEntity>, Task<TResult>> query)
        {
            var result = await _inner.Query(query);

            // Dehydrate if the query result is an entity, otherwise pass through
            return (typeof(TEntity) == typeof(TResult))
                ? (TResult)(object)((TEntity)(object)result)?.DehydrateReferences()
                : result;
        }

        public async Task<IEntity> FindUntypedAsync(long id)
        {
            var entity = await _inner.FindAsync(id);
            return entity?.DehydrateReferences();
        }
#endif
    }
}
