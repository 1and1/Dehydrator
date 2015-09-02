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
        /// Copies all public properties <paramref name="from"/> an <see cref="IEntity"/> to a new clone of it.
        /// Skips <see cref="IEntity.Id"/>.
        /// </summary>
        [NotNull]
        public static TEntity Clone<TEntity>([NotNull] this TEntity from)
            where TEntity : class, IEntity, new()
        {
            var to = new TEntity();
            from.TransferState(to);
            return to;
        }

        /// <summary>
        /// Copies all public properties <paramref name="from"/> one <see cref="IEntity"/> <paramref name="to"/> another.
        /// Skips <see cref="IEntity.Id"/>. Recursivley copies <see cref="IEntity"/> collections.
        /// </summary>
        public static void TransferState<TEntity>([NotNull] this TEntity from, [NotNull] TEntity to)
            where TEntity : class, IEntity, new()
        {
            var entityType = typeof(TEntity);

            foreach (var prop in entityType.GetWritableProperties())
            {
                if (prop.Name == nameof(IEntity.Id)) continue;

                var fromValue = prop.GetValue(@from, null);
                if (prop.IsEntityCollection())
                {
                    if (fromValue == null) continue;
                    var referenceType = prop.GetGenericArg();

                    object targetList = prop.GetValue(to, null);
                    Type collectionType;
                    if (targetList == null)
                    {
                        collectionType = typeof(List<>).MakeGenericType(referenceType);
                        targetList = Activator.CreateInstance(collectionType);
                        prop.SetValue(obj: to, value: targetList, index: null);
                    }
                    else
                    {
                        collectionType = targetList.GetType();
                        collectionType.InvokeClear(target: targetList);
                    }

                    foreach (IEntity reference in (IEnumerable)fromValue)
                    {
                        collectionType.InvokeAdd(target: targetList,
                            value: reference);
                    }
                }
                else prop.SetValue(obj: to, value: fromValue, index: null);
            }
        }

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
        private static IEntity DehydrateOrRecurse([NotNull] this PropertyInfo prop, [NotNull] Type referenceType, [NotNull] IEntity entity)
        {
            if (prop.HasAttribute<DehydrateAttribute>()) return entity.Dehydrate(referenceType);
            else if (prop.HasAttribute<DehydrateReferencesAttribute>()) return entity.DehydrateReferences(referenceType);
            else return entity;
        }

        [NotNull]
        private static IEntity Dehydrate([NotNull] this IEntity entity, [NotNull] Type entityType)
        {
            if (entity.Id == Entity.NoId) return entity;

            var dehydratedRef = (IEntity)Activator.CreateInstance(entityType);
            dehydratedRef.Id = entity.Id;
            return dehydratedRef;
        }

        /// <summary>
        /// Resolves references that were dehydrated by <see cref="DehydrateReferences{TEntity}"/> to the original full entities. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        /// <typeparam name="TEntity">The specific type of the entity.</typeparam>
        /// <seealso cref="RepositoryExtensions.Resolve"/>
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
            [NotNull] IEntity entity, [NotNull] IRepositoryFactory repositoryFactory)
        {
            if (prop.HasAttribute<ResolveAttribute>()) return repositoryFactory.Create(referenceType).Resolve(entity);
            else if (prop.HasAttribute<ResolveReferencesAttribute>()) return entity.ResolveReferences(referenceType, repositoryFactory);
            return entity;
        }

#if NET45
        /// <summary>
        /// Resolves references that were dehydrated by <see cref="DehydrateReferences{TEntity}"/> to the original full entities. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        /// <typeparam name="TEntity">The specific type of the entity.</typeparam>
        /// <seealso cref="RepositoryExtensions.Resolve"/>
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
        private static async Task<IEntity> ResolveOrRecurseAsync([NotNull] this PropertyInfo prop, [NotNull] Type referenceType,
            [NotNull] IEntity entity, [NotNull] IRepositoryFactory repositoryFactory)
        {
            if (prop.HasAttribute<ResolveAttribute>()) return await repositoryFactory.Create(referenceType).ResolveAsync(entity);
            else if (prop.HasAttribute<ResolveReferencesAttribute>()) return await entity.ResolveReferencesAsync(referenceType, repositoryFactory);
            return entity;
        }
#endif
    }
}