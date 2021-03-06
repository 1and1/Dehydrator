﻿using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

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

        [NotNull] private readonly IReadRepositoryFactory _repositoryFactory;

        /// <summary>
        /// Creates a new reference-dehydrating decorator.
        /// </summary>
        /// <param name="inner">The inner repository to use for the actual storage.</param>
        /// <param name="repositoryFactory">Used to aquire additional repositories for resolving references.</param>
        public DehydratingCrudRepository([NotNull] ICrudRepository<TEntity> inner,
            [NotNull] IReadRepositoryFactory repositoryFactory) : base(inner)
        {
            _repositoryFactory = repositoryFactory;
            Inner = inner;
        }

        public TEntity Add(TEntity entity) => Inner.Add(
            entity.ResolveReferences(_repositoryFactory)).DehydrateReferences();

        public void Modify(TEntity entity) => Inner.Modify(
            entity.ResolveReferences(_repositoryFactory));

        public bool Remove(long id) => Inner.Remove(id);

        public ITransaction BeginTransaction() => Inner.BeginTransaction();

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken()) => (await Inner.AddAsync(
            await entity.ResolveReferencesAsync(_repositoryFactory), cancellationToken)).DehydrateReferences();

        public async Task ModifyAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken)) => await Inner.ModifyAsync(
            await entity.ResolveReferencesAsync(_repositoryFactory), cancellationToken);

        public Task<bool> RemoveAsync(long id, CancellationToken cancellationToken = default(CancellationToken)) => Inner.RemoveAsync(id, cancellationToken);

        public Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken)) => Inner.BeginTransactionAsync(cancellationToken);
    }
}