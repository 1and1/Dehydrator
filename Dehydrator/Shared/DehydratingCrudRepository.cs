using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace Dehydrator
{
    /// <summary>
    /// Decorator for <see cref="ICrudRepository{TEntity}"/> instances that transparently dehydrates references on entities it returns and resolves them on entities that are put it.
    /// </summary>
    public class DehydratingCrudRepository<TEntity> : DehydratingReadRepository<TEntity>, ICrudRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// The underlying repository this decorater delegates calls to after performing its own processing.
        /// </summary>
        [NotNull] protected new readonly ICrudRepository<TEntity> Inner;

        [NotNull] private readonly ICrudRepositoryFactory _repositoryFactory;

        /// <summary>
        /// Creates a new reference-dehydrating decorator.
        /// </summary>
        /// <param name="inner">The inner repository to use for the actual storage.</param>
        /// <param name="repositoryFactory">Used to aquire additional repositories for resolving references.</param>
        public DehydratingCrudRepository([NotNull] ICrudRepository<TEntity> inner,
            [NotNull] ICrudRepositoryFactory repositoryFactory) : base(inner)
        {
            _repositoryFactory = repositoryFactory;
            Inner = inner;
        }

        public TEntity Add(TEntity entity)
        {
            return Inner.Add(
                entity.ResolveReferences(_repositoryFactory));
        }

        public void Modify(TEntity entity)
        {
            Inner.Modify(
                entity.ResolveReferences(_repositoryFactory));
        }

        public bool Remove(long id)
        {
            return Inner.Remove(id);
        }

        public ITransaction BeginTransaction()
        {
            return Inner.BeginTransaction();
        }

        public void SaveChanges()
        {
            Inner.SaveChanges();
        }

#if NET45
        public async Task<TEntity> FindAsync(long id)
        {
            var entity = await Inner.FindAsync(id);
            return entity?.DehydrateReferences();
        }

        public async Task ModifyAsync(TEntity entity)
        {
            await Inner.ModifyAsync(
                await entity.ResolveReferencesAsync(_repositoryFactory));
        }

        public Task<bool> RemoveAsync(long id)
        {
            return Inner.RemoveAsync(id);
        }

        public Task<ITransaction> BeginTransactionAsync()
        {
            return Inner.BeginTransactionAsync();
        }

        public Task SaveChangesAsync()
        {
            return Inner.SaveChangesAsync();
        }
#endif
    }
}