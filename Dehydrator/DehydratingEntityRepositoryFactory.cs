using System;
using JetBrains.Annotations;

namespace Dehydrator.WebApi
{
    /// <summary>
    /// Decorator for <see cref="IEntityRepositoryFactory"/> instances that transparently dehydrates references on entities it returns and resolves them on entities that are put it.
    /// </summary>
    [PublicAPI]
    public class DehydratingEntityRepositoryFactory : IEntityRepositoryFactory
    {
        [NotNull] private readonly IEntityRepositoryFactory _inner;

        /// <summary>
        /// Creates a new reference-dehyrdating decorator.
        /// </summary>
        /// <param name="inner">The inner factory to use for the actual storage.</param>
        public DehydratingEntityRepositoryFactory([NotNull] IEntityRepositoryFactory inner)
        {
            _inner = inner;
        }

        public IEntityRepository<TEntity> Create<TEntity>() where TEntity : class, IEntity, new()
        {
            return new DehydratingEntityRepository<TEntity>(_inner.Create<TEntity>(), _inner);
        }

        public IEntityRepository<IEntity> Create(Type entityType)
        {
            return new DehydratingEntityRepository<IEntity>(_inner.Create(entityType), _inner);
        }
    }
}
