using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Stores <see cref="IReadRepository{TEntity}"/> instances and returns them on request rather than creating new instances. Usefull for unit tests.
    /// </summary>
    /// <example>var factory = new LookupReadRepositoryFactory {repo1, repo2};</example>
    [PublicAPI]
    public class LookupReadRepositoryFactory : IReadRepositoryFactory, IEnumerable<object>
    {
        protected readonly Dictionary<Type, object> Repositories = new Dictionary<Type, object>();

        /// <summary>
        /// Registers an <see cref="IReadRepository{TEntity}"/> instance for later retrieval via <see cref="ICrudRepositoryFactory.Create{TEntity}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return the repository for.</typeparam>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        [NotNull]
        public LookupReadRepositoryFactory Add<TEntity>([NotNull] ICrudRepository<TEntity> repository)
            where TEntity : class, IEntity, new()
        {
            Repositories.Add(typeof(TEntity), repository);
            return this;
        }

        public IReadRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return (ICrudRepository<TEntity>)Repositories[typeof(TEntity)];
        }

        // NOTE: Implement IEnumerable<T> to get support for initializer syntax
        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return Repositories.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Repositories.Values.GetEnumerator();
        }
    }
}