using System.Collections.Generic;
using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace EntityReferenceStripper
{
    public static class EntitryRepositoryExtensions
    {
        /// <summary>
        /// Resolves an entity that has been stripped to contain nothing but its <see cref="IEntity.Id"/> and returns the full entity.
        /// </summary>
        /// <param name="repository">The repository to retrieve entities from.</param>
        /// <param name="entity">The entity to resolve.</param>
        /// <exception cref="KeyNotFoundException">No entity with the <see cref="IEntity.Id"/> value from <paramref name="entity"/> found in <paramref name="repository"/>.</exception>
        [Pure, NotNull]
        public static TEntity Resolve<TEntity>([NotNull] this IEntityRepository<TEntity> repository,
            [NotNull] IEntity entity)
            where TEntity : IEntity
        {
            var entityWithResolvedRefs = repository.Find(entity.Id);
            if (entityWithResolvedRefs == null) throw new KeyNotFoundException();
            return entityWithResolvedRefs;
        }

#if NET45
        /// <summary>
        /// Resolves an entity that has been stripped to contain nothing but its <see cref="IEntity.Id"/> and returns the full entity.
        /// </summary>
        /// <param name="repository">The repository to retrieve entities from.</param>
        /// <param name="entity">The entity to resolve.</param>
        /// <exception cref="KeyNotFoundException">No entity with the <see cref="IEntity.Id"/> value from <paramref name="entity"/> found in <paramref name="repository"/>.</exception>
        [Pure, NotNull]
        public static async Task<TEntity> ResolveAsync<TEntity>([NotNull] this IEntityRepository<TEntity> repository,
            [NotNull] IEntity entity)
            where TEntity : IEntity
        {
            var entityWithResolvedRefs = await repository.FindAsync(entity.Id);
            if (entityWithResolvedRefs == null) throw new KeyNotFoundException();
            return entityWithResolvedRefs;
        }
#endif
    }
}