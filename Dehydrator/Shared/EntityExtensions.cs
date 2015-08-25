using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace Dehydrator
{
    /// <summary>
    /// Provides extension methods for <see cref="IEntity"/>s.
    /// </summary>
    [PublicAPI]
    public static class EntityExtensions
    {
        /// <summary>
        /// Dehydrates all references to contain nothing but their <see cref="IEntity.Id"/>s and returns the result as a new object.
        /// </summary>
        /// <param name="entity">The entity to dehydrate.</param>
        [Pure, NotNull]
        public static TEntity DehydrateReferences<TEntity>([NotNull] this TEntity entity)
            where TEntity : class, IEntity, new()
        {
            var entityType = typeof(TEntity);

            var newEntity = (TEntity)Activator.CreateInstance(entityType);
            foreach (var prop in entityType.GetWritableProperties())
            {
                var propertyValue = prop.GetValue(entity, null);
                if (propertyValue == null) continue;

                if (prop.IsEntity())
                {
                    prop.SetValue(obj: newEntity,
                        value: ((IEntity)propertyValue).Dehydrate(prop.PropertyType), index: null);
                }
                else if (prop.IsEntityCollection())
                {
                    var referenceType = prop.GetGenericArg();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    var dehydratedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(obj: newEntity, value: dehydratedRefs, index: null);

                    foreach (IEntity resolvedRef in (IEnumerable)propertyValue)
                    {
                        if (resolvedRef == null) continue;
                        collectionType.InvokeAdd(target: dehydratedRefs,
                            value: resolvedRef.Dehydrate(referenceType));
                    }
                }
                else prop.SetValue(obj: newEntity, value: propertyValue, index: null);
            }
            return newEntity;
        }

        [NotNull]
        private static IEntity Dehydrate([NotNull] this IEntity value, [NotNull] Type type)
        {
            var dehydratedRef = (IEntity)Activator.CreateInstance(type);
            dehydratedRef.Id = value.Id;
            return dehydratedRef;
        }

        /// <summary>
        /// Resolves references that were dehydrated to contain nothing but their <see cref="IEntity.Id"/>s to the original full entities and returns the result as a new object.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        /// <seealso cref="RepositoryExtensions.Resolve{TEntity}"/>
        [Pure, NotNull]
        public static TEntity ResolveReferences<TEntity>([NotNull] this TEntity entity,
            [NotNull] IRepositoryFactory repositoryFactory)
            where TEntity : class, IEntity, new()
        {
            var entityType = typeof(TEntity);

            var newEntity = new TEntity();
            foreach (var prop in entityType.GetWritableProperties())
            {
                var propertyValue = prop.GetValue(entity, null);
                if (propertyValue == null) continue;

                if (prop.IsEntity())
                {
                    var referenceRepository = repositoryFactory.Create(prop.PropertyType);
                    prop.SetValue(obj: newEntity,
                        value: referenceRepository.Resolve((IEntity)propertyValue), index: null);
                }
                else if (prop.IsEntityCollection())
                {
                    var referenceType = prop.GetGenericArg();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    var resolvedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(newEntity, resolvedRefs, null);

                    var referenceRepository = repositoryFactory.Create(referenceType);
                    foreach (IEntity dehydratedRef in (IEnumerable)propertyValue)
                    {
                        if (dehydratedRef == null) continue;
                        collectionType.InvokeAdd(target: resolvedRefs,
                            value: referenceRepository.Resolve(dehydratedRef));
                    }
                }
                else prop.SetValue(obj: newEntity, value: propertyValue, index: null);
            }
            return newEntity;
        }

#if NET45
        /// <summary>
        /// Resolves references that were dehydrated to contain nothing but their <see cref="IEntity.Id"/>s to the original full entities and returns the result as a new object.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        /// <seealso cref="RepositoryExtensions.ResolveAsync{TEntity}"/>
        [Pure, NotNull]
        public static async Task<TEntity> ResolveReferencesAsync<TEntity>([NotNull] this TEntity entity,
            [NotNull] IRepositoryFactory repositoryFactory)
            where TEntity : class, IEntity, new()
        {
            var entityType = typeof(TEntity);

            var newEntity = new TEntity();
            foreach (var prop in entityType.GetWritableProperties())
            {
                var propertyValue = prop.GetValue(entity, null);
                if (propertyValue == null) continue;

                if (prop.IsEntity())
                {
                    var referenceRepository = repositoryFactory.Create(prop.PropertyType);
                    prop.SetValue(obj: newEntity,
                        value: await referenceRepository.ResolveAsync((IEntity)propertyValue), index: null);
                }
                else if (prop.IsEntityCollection())
                {
                    var referenceType = prop.GetGenericArg();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    var resolvedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(newEntity, resolvedRefs, null);

                    var referenceRepository = repositoryFactory.Create(referenceType);
                    foreach (IEntity dehydratedRef in (IEnumerable)propertyValue)
                    {
                        if (dehydratedRef == null) continue;
                        collectionType.InvokeAdd(target: resolvedRefs,
                            value: await referenceRepository.ResolveAsync(dehydratedRef));
                    }
                }
                else prop.SetValue(obj: newEntity, value: propertyValue, index: null);
            }
            return newEntity;
        }
#endif
    }
}