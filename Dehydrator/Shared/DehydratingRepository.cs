using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace Dehydrator
{
    /// <summary>
    /// Decorator for <see cref="IRepository{TEntity}"/> instances that transparently dehydrates references on entities it returns and resolves them on entities that are put it.
    /// </summary>
    public class DehydratingRepository<TEntity> : DehydratingReadRepository<TEntity>, IRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        [NotNull] private readonly IRepositoryFactory _repositoryFactory;
        [NotNull] private readonly IRepository<TEntity> _inner;

        /// <summary>
        /// Creates a new reference-dehydrating decorator.
        /// </summary>
        /// <param name="repositoryFactory">Used to aquire additional repositories for resolving references.</param>
        /// <param name="inner">The inner repository to use for the actual storage.</param>
        public DehydratingRepository([NotNull] IRepositoryFactory repositoryFactory,
            [NotNull] IRepository<TEntity> inner) : base(inner)
        {
            _repositoryFactory = repositoryFactory;
            _inner = inner;
        }

        /// <summary>
        /// Creates a new reference-dehydrating decorator.
        /// </summary>
        /// <param name="repositoryFactory">Used to aquire the repository for <typeparamref name="TEntity"/> and repositories for resolving references.</param>
        public DehydratingRepository([NotNull] IRepositoryFactory repositoryFactory)
            : this(repositoryFactory, repositoryFactory.Create<TEntity>())
        {
        }

        public TEntity Add(TEntity entity)
        {
            return _inner.Add(entity.ResolveReferences(_repositoryFactory));
        }

        public void Modify(TEntity entity)
        {
            _inner.Modify(entity.ResolveReferences(_repositoryFactory));
        }

        public bool Remove(long id)
        {
            return _inner.Remove(id);
        }

#if NET45
        public async Task<TEntity> FindAsync(long id)
        {
            var entity = await _inner.FindAsync(id);
            return entity?.DehydrateReferences();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            return _inner.Add(await entity.ResolveReferencesAsync(_repositoryFactory));
        }

        public async Task ModifyAsync(TEntity entity)
        {
            await _inner.ModifyAsync(await entity.ResolveReferencesAsync(_repositoryFactory));
        }

        public Task<bool> RemoveAsync(long id)
        {
            return _inner.RemoveAsync(id);
        }
#endif
    }
}