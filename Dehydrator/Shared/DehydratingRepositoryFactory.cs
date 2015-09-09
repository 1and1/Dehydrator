using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Decorator for <see cref="ICrudRepositoryFactory"/> instances that transparently dehydrates references on entities it returns and resolves them on entities that are put it.
    /// </summary>
    [PublicAPI]
    public class DehydratingRepositoryFactory : DehydratingReadRepositoryFactory, ICrudRepositoryFactory
    {
        [NotNull] private readonly ICrudRepositoryFactory _inner;

        /// <summary>
        /// Creates a new reference-dehyrdating decorator.
        /// </summary>
        /// <param name="inner">The inner factory to use for the actual storage.</param>
        public DehydratingRepositoryFactory([NotNull] ICrudRepositoryFactory inner)
            : base(inner)
        {
            _inner = inner;
        }

        public new ICrudRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return new DehydratingCrudRepository<TEntity>(_inner);
        }
    }
}