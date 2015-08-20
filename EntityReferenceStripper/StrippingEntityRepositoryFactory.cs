using System;
using JetBrains.Annotations;

namespace EntityReferenceStripper.WebApi
{
    /// <summary>
    /// Decorator for <see cref="IEntityRepositoryFactory"/> instances that transparently strips references on entities it returns and resolves them on entities that are put it.
    /// </summary>
    [PublicAPI]
    public class StrippingEntityRepositoryFactory : IEntityRepositoryFactory
    {
        [NotNull] private readonly IEntityRepositoryFactory _inner;

        /// <summary>
        /// Creates a new reference-stripping decorator.
        /// </summary>
        /// <param name="inner">The inner factory to use for the actual storage.</param>
        public StrippingEntityRepositoryFactory([NotNull] IEntityRepositoryFactory inner)
        {
            _inner = inner;
        }

        public IEntityRepository<TEntity> Create<TEntity>() where TEntity : class, IEntity, new()
        {
            return new StrippingEntityRepository<TEntity>(_inner.Create<TEntity>(), _inner);
        }

        public IEntityRepository<IEntity> Create(Type entityType)
        {
            return new StrippingEntityRepository<IEntity>(_inner.Create(entityType), _inner);
        }
    }
}