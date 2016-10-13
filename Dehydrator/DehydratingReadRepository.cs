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

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return Inner.GetAll(predicate).Select(x => x.Dehydrate());
        }

        public TEntity Find(long id)
        {
            return Inner.Find(id)?.DehydrateReferences();
        }

        public bool Exists(long id)
        {
            return Inner.Exists(id);
        }

        public IQueryable<TEntity> Query => new DehydratingQueryable<TEntity>(Inner.Query);

        public async Task<TEntity> FindAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entity = await Inner.FindAsync(id, cancellationToken);
            return entity?.DehydrateReferences();
        }
    }
}