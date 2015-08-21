using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dehydrator
{
    public static class EntitryRepositoryExtensions
    {
        /// <summary>
        /// Resolves an <see cref="IEntity"/> that has been stripped to contain nothing but its <see cref="IEntity.Id"/> and returns the full entity.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="entityType">The concrete type of <paramref name="entity"/>.</param>
        [Pure, NotNull]
        public static IEntity Resolve<TEntity>([NotNull] this IEntityRepository<TEntity> repository, [NotNull] IEntity entity, [NotNull] Type entityType)
            where TEntity : IEntity
        {
            var entityWithResolvedRefs = repository.Find(entity.Id);
        }

        //public IEntity Resolve(IEntity entity, Type entityType)
        //{
        //    return (IEntity)_db.Set(entityType).Find(entity.Id);
        //}

        //public async Task<IEntity> ResolveAsync(IEntity entity, Type entityType)
        //{
        //    var result = await _db.Set(entityType).FindAsync(entity.Id);
        //    return (IEntity)result;
        //}

#if NET45
        /// <summary>
        /// Resolves an <see cref="IEntity"/> that has been stripped to contain nothing but its <see cref="IEntity.Id"/> and returns the full entity.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="entityType">The concrete type of <paramref name="entity"/>.</param>
        [Pure, NotNull]
        public static Task<IEntity> ResolveAsync<TEntity>([NotNull] this IEntityRepository<TEntity> repository, [NotNull] IEntity entity, [NotNull] Type entityType)
            where TEntity : IEntity
        {
            
        }
#endif
    }
}
