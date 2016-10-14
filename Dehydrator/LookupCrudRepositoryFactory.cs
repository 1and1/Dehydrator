using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Stores <see cref="ICrudRepository{TEntity}"/> instances and returns them on request rather than creating new instances. Usefull for unit tests.
    /// </summary>
    /// <example>var factory = new LookupCrudRepositoryFactory {repo1, repo2};</example>
    [PublicAPI]
    public class LookupCrudRepositoryFactory : LookupReadRepositoryFactory, ICrudRepositoryFactory
    {
        /// <summary>
        /// Registers an <see cref="ICrudRepository{TEntity}"/> instance for later retrieval via <see cref="ICrudRepositoryFactory.Create{TEntity}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return the repository for.</typeparam>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        [NotNull]
        public new LookupCrudRepositoryFactory Add<TEntity>([NotNull] ICrudRepository<TEntity> repository)
            where TEntity : class, IEntity, new()
        {
            Repositories.Add(typeof(TEntity), repository);
            return this;
        }

        public new ICrudRepository<TEntity> Create<TEntity>() where TEntity : class, IEntity, new() =>
            (ICrudRepository<TEntity>)Repositories[typeof(TEntity)];
    }
}