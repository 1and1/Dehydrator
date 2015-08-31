using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        /// Dehydrates all references marked with <see cref="DehydrateAttribute"/> to contain nothing but their <see cref="IEntity.Id"/>s. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="entity">The entity to dehydrate.</param>
        /// <typeparam name="TEntity">The specific type of the entity.</typeparam>
        [Pure, NotNull]
        public static TEntity DehydrateReferences<TEntity>([NotNull] this TEntity entity)
            where TEntity : class, IEntity, new()
        {
            return (TEntity)DehydrateReferences(entity, typeof(TEntity));
        }

        /// <summary>
        /// Dehydrates all references marked with <see cref="DehydrateAttribute"/> to contain nothing but their <see cref="IEntity.Id"/>s. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="entity">The entity to dehydrate.</param>
        /// <param name="entityType">The specific type of the entity.</param>
        [Pure, NotNull]
        public static IEntity DehydrateReferences([NotNull] this IEntity entity, [NotNull] Type entityType)
        {
            var newEntity = (IEntity)Activator.CreateInstance(entityType);
            foreach (var prop in entityType.GetWritableProperties())
            {
                var propertyValue = prop.GetValue(entity, null);
                if (propertyValue == null) continue;

                if (prop.IsEntity())
                {
                    var referenceType = prop.PropertyType;
                    var resolvedRef = (IEntity)propertyValue;
                    prop.SetValue(obj: newEntity,
                        value: prop.DehydrateOrRecurse(referenceType, resolvedRef),
                        index: null);
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
                        collectionType.InvokeAdd(
                            target: dehydratedRefs,
                            value: prop.DehydrateOrRecurse(referenceType, resolvedRef));
                    }
                }
                else prop.SetValue(obj: newEntity, value: propertyValue, index: null);
            }
            return newEntity;
        }

        [NotNull]
        private static IEntity DehydrateOrRecurse([NotNull] this PropertyInfo prop, [NotNull] Type referenceType, [NotNull] IEntity resolvedRef)
        {
            return prop.IsMarkedToDehydrate()
                ? resolvedRef.Dehydrate(referenceType)
                : resolvedRef.DehydrateReferences(referenceType);
        }

        [NotNull]
        private static IEntity Dehydrate([NotNull] this IEntity value, [NotNull] Type entityType)
        {
            var dehydratedRef = (IEntity)Activator.CreateInstance(entityType);
            dehydratedRef.Id = value.Id;
            return dehydratedRef;
        }

        /// <summary>
        /// Resolves references that were dehydrated by <see cref="DehydrateReferences{TEntity}"/> to the original full entities. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        /// <typeparam name="TEntity">The specific type of the entity.</typeparam>
        /// <seealso cref="RepositoryExtensions.Resolve{TEntity}"/>
        [Pure, NotNull]
        public static TEntity ResolveReferences<TEntity>([NotNull] this TEntity entity,
            [NotNull] IRepositoryFactory repositoryFactory)
            where TEntity : class, IEntity, new()
        {
            return (TEntity)ResolveReferences(entity, typeof(TEntity), repositoryFactory);
        }

        /// <summary>
        /// Resolves references that were dehydrated by <see cref="DehydrateReferences{TEntity}"/> to the original full entities. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="entityType">The specific type of the entity.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        [Pure, NotNull]
        public static IEntity ResolveReferences([NotNull] this IEntity entity, [NotNull] Type entityType,
            [NotNull] IRepositoryFactory repositoryFactory)
        {
            var newEntity = (IEntity)Activator.CreateInstance(entityType);
            foreach (var prop in entityType.GetWritableProperties())
            {
                var propertyValue = prop.GetValue(entity, null);
                if (propertyValue == null) continue;

                if (prop.IsEntity())
                {
                    var referenceType = prop.PropertyType;
                    var dehydratedRef = (IEntity)propertyValue;

                    prop.SetValue(
                        obj: newEntity,
                        value: prop.ResolveOrRecurse(referenceType, dehydratedRef, repositoryFactory),
                        index: null);
                }
                else if (prop.IsEntityCollection())
                {
                    var referenceType = prop.GetGenericArg();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    var resolvedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(newEntity, resolvedRefs, null);

                    foreach (IEntity dehydratedRef in (IEnumerable)propertyValue)
                    {
                        if (dehydratedRef == null) continue;

                        collectionType.InvokeAdd(
                            target: resolvedRefs,
                            value: prop.ResolveOrRecurse(referenceType, dehydratedRef, repositoryFactory));
                    }
                }
                else prop.SetValue(obj: newEntity, value: propertyValue, index: null);
            }
            return newEntity;
        }

        [NotNull]
        private static IEntity ResolveOrRecurse([NotNull] this PropertyInfo prop, [NotNull] Type referenceType,
            [NotNull] IEntity dehydratedRef, [NotNull] IRepositoryFactory repositoryFactory)
        {
            return prop.IsMarkedToDehydrate()
                ? repositoryFactory.Create(referenceType).Resolve(dehydratedRef)
                : dehydratedRef.ResolveReferences(referenceType, repositoryFactory);
        }

#if NET45
        /// <summary>
        /// Resolves references that were dehydrated by <see cref="DehydrateReferences{TEntity}"/> to the original full entities. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        /// <typeparam name="TEntity">The specific type of the entity.</typeparam>
        /// <seealso cref="RepositoryExtensions.Resolve{TEntity}"/>
        [Pure, NotNull]
        public static async Task<TEntity> ResolveReferencesAsync<TEntity>([NotNull] this TEntity entity,
            [NotNull] IRepositoryFactory repositoryFactory)
            where TEntity : class, IEntity, new()
        {
            return (TEntity)await ResolveReferencesAsync(entity, typeof(TEntity), repositoryFactory);
        }

        /// <summary>
        /// Resolves references that were dehydrated by <see cref="DehydrateReferences{TEntity}"/> to the original full entities. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="entityType">The specific type of the entity.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        [Pure, NotNull]
        public static async Task<IEntity> ResolveReferencesAsync([NotNull] this IEntity entity, [NotNull] Type entityType,
            [NotNull] IRepositoryFactory repositoryFactory)
        {
            var newEntity = (IEntity)Activator.CreateInstance(entityType);
            foreach (var prop in entityType.GetWritableProperties())
            {
                var propertyValue = prop.GetValue(entity, null);
                if (propertyValue == null) continue;

                if (prop.IsEntity())
                {
                    var referenceType = prop.PropertyType;
                    var dehydratedRef = (IEntity)propertyValue;

                    prop.SetValue(
                        obj: newEntity,
                        value: await prop.ResolveOrRecurseAsync(referenceType, dehydratedRef, repositoryFactory),
                        index: null);
                }
                else if (prop.IsEntityCollection())
                {
                    var referenceType = prop.GetGenericArg();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    var resolvedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(newEntity, resolvedRefs, null);

                    foreach (IEntity dehydratedRef in (IEnumerable)propertyValue)
                    {
                        if (dehydratedRef == null) continue;

                        collectionType.InvokeAdd(
                            target: resolvedRefs,
                            value: await prop.ResolveOrRecurseAsync(referenceType, dehydratedRef, repositoryFactory));
                    }
                }
                else prop.SetValue(obj: newEntity, value: propertyValue, index: null);
            }
            return newEntity;
        }

        [NotNull]
        private static Task<IEntity> ResolveOrRecurseAsync([NotNull] this PropertyInfo prop, [NotNull] Type referenceType,
            [NotNull] IEntity dehydratedRef, [NotNull] IRepositoryFactory repositoryFactory)
        {
            return prop.IsMarkedToDehydrate()
                ? repositoryFactory.Create(referenceType).ResolveAsync(dehydratedRef)
                : dehydratedRef.ResolveReferencesAsync(referenceType, repositoryFactory);
        }
#endif
    }
}