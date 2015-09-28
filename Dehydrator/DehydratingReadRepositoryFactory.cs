using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Decorator for <see cref="IReadRepositoryFactory"/> instances that transparently dehydrates references on entities it returns and resolves them on entities that are put it.
    /// </summary>
    [PublicAPI]
    public class DehydratingReadRepositoryFactory : IReadRepositoryFactory
    {
        [NotNull] private readonly IReadRepositoryFactory _inner;

        /// <summary>
        /// Creates a new reference-dehyrdating decorator.
        /// </summary>
        /// <param name="inner">The inner factory to use for the actual storage.</param>
        public DehydratingReadRepositoryFactory([NotNull] IReadRepositoryFactory inner)
        {
            _inner = inner;
        }

        public IReadRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return new DehydratingReadRepository<TEntity>(_inner.Create<TEntity>());
        }
    }
}