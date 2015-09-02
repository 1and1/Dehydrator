using System.Collections.Generic;
using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace Dehydrator
{
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Resolves an entity that has been dehydrated to contain nothing but its <see cref="IEntity.Id"/> and returns the full entity.
        /// </summary>
        /// <param name="repository">The repository to retrieve entities from.</param>
        /// <param name="entity">The entity to resolve.</param>
        /// <exception cref="KeyNotFoundException">No entity with matching <see cref="IEntity.Id"/> in <paramref name="repository"/>.</exception>
        [Pure, NotNull]
        public static IEntity Resolve([NotNull] this IReadRepository<IEntity> repository,
            [NotNull] IEntity entity)
        {
            var entityWithResolvedRefs = repository.Find(entity.Id);
            if (entityWithResolvedRefs == null)
                throw new KeyNotFoundException($"{entity.GetType().Name} with ID {entity.Id} not found.");
            return entityWithResolvedRefs;
        }

#if NET45
        /// <summary>
        /// Resolves an entity that has been dehydrated to contain nothing but its <see cref="IEntity.Id"/> and returns the full entity.
        /// </summary>
        /// <param name="repository">The repository to retrieve entities from.</param>
        /// <param name="entity">The entity to resolve.</param>
        /// <exception cref="KeyNotFoundException">No entity with matching <see cref="IEntity.Id"/> in <paramref name="repository"/>.</exception>
        [Pure, NotNull]
        public static async Task<IEntity> ResolveAsync([NotNull] this IReadRepository<IEntity> repository,
            [NotNull] IEntity entity)
        {
            var entityWithResolvedRefs = await repository.FindAsync(entity.Id);
            if (entityWithResolvedRefs == null)
                throw new KeyNotFoundException($"{entity.GetType().Name} with ID {entity.Id} not found.");
            return entityWithResolvedRefs;
        }

        /// <summary>
        /// Returns a specific entity from the backing database.
        /// </summary>
        /// <param name="repository">The repository to retrieve entities from.</param>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to find.</param>
        /// <returns>The entity or <see langword="null"/> if there was no match.</returns>
        /// <remarks>This is a type-safe wrapper for <see cref="IReadRepository{TEntity}.FindUntypedAsync"/>.</remarks>
        public static async Task<TEntity> FindAsync<TEntity>([NotNull] this IReadRepository<TEntity> repository, long id)
            where TEntity : class, IEntity
        {
            return (TEntity)await repository.FindUntypedAsync(id);
        }
#endif
    }
}
