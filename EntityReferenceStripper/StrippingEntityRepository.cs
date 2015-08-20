using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace EntityReferenceStripper
{
    /// <summary>
    /// Decorator for <see cref="IEntityRepository{TEntity}"/> instances that transparently strips references on entities it returns and resolves them on entities that are put it.
    /// </summary>
    internal class StrippingEntityRepository<TEntity> : IEntityRepository<TEntity>
        where TEntity : class, IEntity
    {
        [NotNull] private readonly IEntityRepository<TEntity> _inner;
        [NotNull] private readonly IEntityRepositoryFactory _repositoryFactory;

        /// <summary>
        /// Creates a new reference-stripping decorator.
        /// </summary>
        /// <param name="inner">The inner repository to use for the actual storage.</param>
        /// <param name="repositoryFactory">Used to aquire additional repositories for resolving references.</param>
        public StrippingEntityRepository([NotNull] IEntityRepository<TEntity> inner,
            [NotNull] IEntityRepositoryFactory repositoryFactory)
        {
            _inner = inner;
            _repositoryFactory = repositoryFactory;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _inner.GetAll().Select(x => x.StripReferences());
        }

        public TEntity Find(int id)
        {
            return _inner.Find(id)?.StripReferences();
        }

        public void Modify(TEntity entity)
        {
            _inner.Modify(entity.ResolveReferences(_repositoryFactory));
        }

        public bool Exists(int id)
        {
            return _inner.Exists(id);
        }

        public TEntity Add(TEntity entity)
        {
            return _inner.Add(entity.ResolveReferences(_repositoryFactory));
        }

        public bool Remove(int id)
        {
            return _inner.Remove(id);
        }

#if NET45
        public async Task<TEntity> FindAsync(int id)
        {
            var entity = await _inner.FindAsync(id);
            return entity?.StripReferences();
        }

        public async Task ModifyAsync(TEntity entity)
        {
            await _inner.ModifyAsync(await entity.ResolveReferencesAsync(_repositoryFactory));
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            return _inner.Add(await entity.ResolveReferencesAsync(_repositoryFactory));
        }

        public Task<bool> RemoveAsync(int id)
        {
            return _inner.RemoveAsync(id);
        }
#endif
    }
}