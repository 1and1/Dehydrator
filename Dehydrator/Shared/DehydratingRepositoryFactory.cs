using JetBrains.Annotations;

namespace Dehydrator.WebApi
{
    /// <summary>
    /// Decorator for <see cref="IRepositoryFactory"/> instances that transparently dehydrates references on entities it returns and resolves them on entities that are put it.
    /// </summary>
    [PublicAPI]
    public class DehydratingRepositoryFactory : IRepositoryFactory
    {
        [NotNull] private readonly IRepositoryFactory _inner;

        /// <summary>
        /// Creates a new reference-dehyrdating decorator.
        /// </summary>
        /// <param name="inner">The inner factory to use for the actual storage.</param>
        public DehydratingRepositoryFactory([NotNull] IRepositoryFactory inner)
        {
            _inner = inner;
        }

        public IRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return new DehydratingRepository<TEntity>(_inner);
        }
    }
}
