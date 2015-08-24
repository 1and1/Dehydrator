using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Stores <see cref="IRepository{TEntity}"/> instances and returns them on request rather than creating new instances. Usefull for unit tests.
    /// </summary>
    /// <example>var factory = new LookupRepositoryFactory {repo1, repo2};</example>
    [PublicAPI]
    public class LookupRepositoryFactory : IRepositoryFactory, IEnumerable<object>
    {
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        /// <summary>
        /// Registers an <see cref="IRepository{TEntity}"/> instance for later retrieval via <see cref="IRepositoryFactory.Create{TEntity}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return the repository for.</typeparam>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        [NotNull]
        public LookupRepositoryFactory Add<TEntity>([NotNull] IRepository<TEntity> repository)
            where TEntity : class, IEntity, new()
        {
            _repositories.Add(typeof(TEntity), repository);
            return this;
        }

        public IRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return (IRepository<TEntity>)_repositories[typeof(TEntity)];
        }

        // NOTE: Implement IEnumerable<T> to get support for initializer syntax
        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return _repositories.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _repositories.Values.GetEnumerator();
        }
    }
}
