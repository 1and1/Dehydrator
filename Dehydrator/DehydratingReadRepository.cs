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
        /// <summary>
        /// The underlying repository this decorater delegates calls to after performing its own processing.
        /// </summary>
        [NotNull] protected readonly IReadRepository<TEntity> Inner;

        /// <summary>
        /// Creates a new reference-dehydrating decorator.
        /// </summary>
        /// <param name="inner">The inner repository to use for the actual storage.</param>
        public DehydratingReadRepository([NotNull] IReadRepository<TEntity> inner)
        {
            Inner = inner;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Inner.GetAll().Select(x => x.Dehydrate());
        }

        public IEnumerable<TResult> GetAll<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query)
        {
            var result = Inner.GetAll(query);

            // Dehydrate if the query result is a collection of entities, otherwise pass through
            return (typeof(TEntity) == typeof(TResult))
                ? result.Cast<TEntity>().Select(x => x.Dehydrate()).Cast<TResult>()
                : result;
        }

        public TEntity Find(long id)
        {
            return Inner.Find(id)?.DehydrateReferences();
        }

        public bool Exists(long id)
        {
            return Inner.Exists(id);
        }

#if NET45
        public async Task<IReadOnlyCollection<TResult>> QueryAsync<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query)
        {
            var result = await Inner.QueryAsync(query);
            return result.Select(x => x.DehydrateReferences()).ToList();
        }

        public async Task<TResult> FirstAsync<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query)
        {
            var result = await Inner.FirstAsync(query);

            // Dehydrate if the query result is an entity, otherwise pass through
            return IsEntity(typeof(TResult))
                ? (TResult)(object)((TEntity)(object)result)?.DehydrateReferences()
                : result;
        }

        public async Task<TResult> FirstOrDefaultAsync<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query)
        {
            var result = await Inner.FirstOrDefaultAsync(query);

            // Dehydrate if the query result is an entity, otherwise pass through
            return IsEntity(typeof(TResult))
                ? (TResult)(object)((TEntity)(object)result)?.DehydrateReferences()
                : result;
        }

        private static bool IsEntity(Type type)
        {
            return type == typeof(IEntity) || type.GetInterfaces().Contains(typeof(IEntity));
        }

        public async Task<TEntity> FindAsync(long id)
        {
            var entity = await Inner.FindAsync(id);
            return entity?.DehydrateReferences();
        }
#endif
    }
}