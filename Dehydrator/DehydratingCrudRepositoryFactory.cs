﻿using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Decorator for <see cref="ICrudRepositoryFactory"/> instances that transparently dehydrates references on entities it returns and resolves them on entities that are put it.
    /// </summary>
    [PublicAPI]
    public class DehydratingCrudRepositoryFactory : DehydratingReadRepositoryFactory, ICrudRepositoryFactory
    {
        [NotNull] private readonly ICrudRepositoryFactory _inner;

        /// <summary>
        /// Creates a new reference-dehydrating decorator.
        /// </summary>
        /// <param name="inner">The inner factory to use for the actual storage.</param>
        public DehydratingCrudRepositoryFactory([NotNull] ICrudRepositoryFactory inner)
            : base(inner)
        {
            _inner = inner;
        }

        public new ICrudRepository<TEntity> Create<TEntity>() where TEntity : class, IEntity, new() =>
            new DehydratingCrudRepository<TEntity>(_inner.Create<TEntity>(), _inner);
    }
}