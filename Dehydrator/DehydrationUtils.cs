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
    /// Provides methods dehydrating and resolving <see cref="IEntity"/>s.
    /// </summary>
    [PublicAPI]
    public static class DehydrationUtils
    {
        /// <summary>
        /// Dehydrates all references marked with <see cref="DehydrateAttribute"/> to contain nothing but their <see cref="IEntity.Id"/>s. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="obj">The entity to dehydrate.</param>
        /// <typeparam name="T">The specific type of the <paramref name="obj"/>.</typeparam>
        [Pure, NotNull]
        public static T DehydrateReferences<T>([NotNull] this T obj)
        {
            return (T)DehydrateReferences(obj, typeof(T));
        }

        /// <summary>
        /// Dehydrates all references marked with <see cref="DehydrateAttribute"/> to contain nothing but their <see cref="IEntity.Id"/>s. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="obj">The object containing references to dehydrate.</param>
        /// <param name="type">The specific type of the <paramref name="obj"/>.</param>
        [Pure, NotNull]
        private static object DehydrateReferences([NotNull] this object obj, [NotNull] Type type)
        {
            if (!type.IsPoco()) return obj;

            var newObj = Activator.CreateInstance(type);
            foreach (var prop in type.GetWritableProperties())
            {
                var propertyValue = prop.GetValue(obj, null);
                if (propertyValue == null) continue;

                if (prop.IsCollection())
                {
                    var referenceType = prop.GetGenericArg();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    object dehydratedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(obj: newObj, value: dehydratedRefs, index: null);

                    dynamic dehydratedRefsDynamic = dehydratedRefs;
                    foreach (object resolvedRef in (IEnumerable)propertyValue)
                    {
                        if (resolvedRef != null)
                            dehydratedRefsDynamic.Add((dynamic)prop.DehydrateOrRecurse(referenceType, resolvedRef));
                    }
                }
                else
                {
                    var referenceType = prop.PropertyType;
                    var resolvedRef = propertyValue;
                    prop.SetValue(obj: newObj,
                        value: prop.DehydrateOrRecurse(referenceType, resolvedRef),
                        index: null);
                }
            }
            return newObj;
        }

        [NotNull]
        private static object DehydrateOrRecurse([NotNull] this PropertyInfo prop, [NotNull] Type referenceType,
            [NotNull] object obj)
        {
            if (prop.HasAttribute<DehydrateAttribute>()) return Dehydrate((IEntity)obj, referenceType);
            else if (prop.HasAttribute<DehydrateReferencesAttribute>()) return obj.DehydrateReferences(referenceType);
            else return obj;
        }

        /// <summary>
        /// Dehydrates the <paramref name="entity"/> to contain nothing but its <see cref="IEntity.Id"/>s (and <see cref="INamedEntity.Name"/> if present).
        /// Returns the result as a new object keeping the original unchanged.
        /// </summary>
        [NotNull]
        public static TEntity Dehydrate<TEntity>([NotNull] this TEntity entity)
            where TEntity : class, IEntity, new()
        {
            return (TEntity)entity.Dehydrate(typeof(TEntity));
        }

        [NotNull]
        private static IEntity Dehydrate([NotNull] this IEntity entity, [NotNull] Type entityType)
        {
            if (entity.Id == Entity.NoId) return entity;

            var dehydratedRef = (IEntity)Activator.CreateInstance(entityType);
            dehydratedRef.Id = entity.Id;

            var namedEntity = entity as INamedEntity;
            if (namedEntity != null)
                ((INamedEntity)dehydratedRef).Name = namedEntity.Name;

            return dehydratedRef;
        }

        /// <summary>
        /// Resolves references that were dehydrated by <see cref="DehydrateReferences{TEntity}"/> to the original full entities. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="obj">The object containing the references to resolve.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        /// <typeparam name="T">The specific type of the <paramref name="obj"/>.</typeparam>
        [Pure, NotNull]
        public static T ResolveReferences<T>([NotNull] this T obj,
            [NotNull] IReadRepositoryFactory repositoryFactory)
        {
            return (T)ResolveReferences(obj, typeof(T), repositoryFactory);
        }

        /// <summary>
        /// Resolves references that were dehydrated by <see cref="DehydrateReferences{TEntity}"/> to the original full entities. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="obj">The object containing the references to resolve.</param>
        /// <param name="type">The specific type of the <paramref name="obj"/>.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        [Pure, NotNull]
        private static object ResolveReferences([NotNull] this object obj, [NotNull] Type type,
            [NotNull] IReadRepositoryFactory repositoryFactory)
        {
            if (!type.IsPoco()) return obj;

            var newObj = Activator.CreateInstance(type);
            foreach (var prop in type.GetWritableProperties())
            {
                var propertyValue = prop.GetValue(obj, null);
                if (propertyValue == null) continue;

                if (prop.IsCollection())
                {
                    var referenceType = prop.GetGenericArg();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    object resolvedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(newObj, resolvedRefs, null);

                    dynamic resolvedRefsDynamic = resolvedRefs;
                    foreach (object dehydratedRef in (IEnumerable)propertyValue)
                    {
                        if (dehydratedRef != null)
                        {
                            resolvedRefsDynamic.Add((dynamic)
                                prop.ResolveOrRecurse(referenceType, dehydratedRef, repositoryFactory));
                        }
                    }
                }
                else
                {
                    var referenceType = prop.PropertyType;
                    var dehydratedRef = propertyValue;

                    prop.SetValue(
                        obj: newObj,
                        value: prop.ResolveOrRecurse(referenceType, dehydratedRef, repositoryFactory),
                        index: null);
                }
            }
            return newObj;
        }

        [NotNull]
        private static object ResolveOrRecurse([NotNull] this PropertyInfo prop, [NotNull] Type referenceType,
            [NotNull] object obj, [NotNull] IReadRepositoryFactory repositoryFactory)
        {
            if (prop.HasAttribute<ResolveAttribute>())
                return DehydrationUtils.Resolve((dynamic)obj, (dynamic)repositoryFactory.Create(referenceType));
            else if (prop.HasAttribute<ResolveReferencesAttribute>())
                return obj.ResolveReferences(referenceType, repositoryFactory);
            return obj;
        }

        /// <summary>
        /// Resolves an entity that has been dehydrated to contain nothing but its <see cref="IEntity.Id"/> and returns the full entity.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="repository">The repository to retrieve entities from.</param>
        /// <exception cref="KeyNotFoundException">No entity with matching <see cref="IEntity.Id"/> in <paramref name="repository"/>.</exception>
        [Pure, NotNull]
        public static TEntity Resolve<TEntity>([NotNull] this TEntity entity,
            [NotNull] IReadRepository<TEntity> repository)
            where TEntity : class, IEntity
        {
            if (entity.Id == Entity.NoId) return entity;

            var entityWithResolvedRefs = repository.Find(entity.Id);
            if (entityWithResolvedRefs == null)
                throw new KeyNotFoundException($"{entity.GetType().Name} with ID {entity.Id} not found.");
            return entityWithResolvedRefs;
        }

#if NET45
        /// <summary>
        /// Resolves references that were dehydrated by <see cref="DehydrateReferences{TEntity}"/> to the original full entities. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="obj">The object containing the references to resolve.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        /// <typeparam name="T">The specific type of the <paramref name="obj"/>.</typeparam>
        [Pure, ItemNotNull]
        public static async Task<T> ResolveReferencesAsync<T>([NotNull] this T obj,
            [NotNull] IReadRepositoryFactory repositoryFactory)
        {
            return (T)await ResolveReferencesAsync(obj, typeof(T), repositoryFactory);
        }

        /// <summary>
        /// Resolves references that were dehydrated by <see cref="DehydrateReferences{TEntity}"/> to the original full entities. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="obj">The object containing the references to resolve.</param>
        /// <param name="type">The specific type of the <paramref name="obj"/>.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        [Pure, NotNull]
        private static async Task<object> ResolveReferencesAsync([NotNull] this object obj, [NotNull] Type type,
            [NotNull] IReadRepositoryFactory repositoryFactory)
        {
            if (!type.IsPoco()) return obj;

            var newObj = Activator.CreateInstance(type);
            foreach (var prop in type.GetWritableProperties())
            {
                var propertyValue = prop.GetValue(obj, null);
                if (propertyValue == null) continue;

                if (prop.IsCollection())
                {
                    var referenceType = prop.GetGenericArg();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    object resolvedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(newObj, resolvedRefs, null);

                    dynamic resolvedRefsDynamic = resolvedRefs;
                    foreach (object dehydratedRef in (IEnumerable)propertyValue)
                    {
                        if (dehydratedRef != null)
                        {
                            resolvedRefsDynamic.Add((dynamic)
                                await prop.ResolveOrRecurseAsync(referenceType, dehydratedRef, repositoryFactory));
                        }
                    }
                }
                else
                {
                    var referenceType = prop.PropertyType;
                    var dehydratedRef = propertyValue;

                    prop.SetValue(
                        obj: newObj,
                        value: await prop.ResolveOrRecurseAsync(referenceType, dehydratedRef, repositoryFactory),
                        index: null);
                }
            }
            return newObj;
        }

        [ItemNotNull]
        private static async Task<object> ResolveOrRecurseAsync([NotNull] this PropertyInfo prop,
            [NotNull] Type referenceType, [NotNull] object obj, [NotNull] IReadRepositoryFactory repositoryFactory)
        {
            if (prop.HasAttribute<ResolveAttribute>())
                return await DehydrationUtils.ResolveAsync((dynamic)obj, (dynamic)repositoryFactory.Create(referenceType));
            else if (prop.HasAttribute<ResolveReferencesAttribute>())
                return await obj.ResolveReferencesAsync(referenceType, repositoryFactory);
            return obj;
        }

        /// <summary>
        /// Resolves an entity that has been dehydrated to contain nothing but its <see cref="IEntity.Id"/> and returns the full entity.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="repository">The repository to retrieve entities from.</param>
        /// <exception cref="KeyNotFoundException">No entity with matching <see cref="IEntity.Id"/> in <paramref name="repository"/>.</exception>
        [Pure, NotNull]
        public static async Task<TEntity> ResolveAsync<TEntity>([NotNull] this TEntity entity,
            [NotNull] IReadRepository<TEntity> repository)
            where TEntity : class, IEntity
        {
            if (entity.Id == Entity.NoId) return entity;

            var entityWithResolvedRefs = await repository.FindAsync(entity.Id);
            if (entityWithResolvedRefs == null)
                throw new KeyNotFoundException($"{entity.GetType().Name} with ID {entity.Id} not found.");
            return entityWithResolvedRefs;
        }
#endif
    }
}