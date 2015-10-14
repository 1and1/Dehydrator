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
                if (prop.IsCollection())
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

                    foreach (object reference in (IEnumerable)fromValue)
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
        public static object DehydrateReferences([NotNull] this object obj, [NotNull] Type type)
        {
            var newObj = Activator.CreateInstance(type);
            foreach (var prop in type.GetWritableProperties())
            {
                var propertyValue = prop.GetValue(obj, null);
                if (propertyValue == null) continue;

                if (prop.IsCollection())
                {
                    var referenceType = prop.GetGenericArg();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    var dehydratedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(obj: newObj, value: dehydratedRefs, index: null);

                    foreach (object resolvedRef in (IEnumerable)propertyValue)
                    {
                        if (resolvedRef == null) continue;
                        collectionType.InvokeAdd(
                            target: dehydratedRefs,
                            value: prop.DehydrateOrRecurse(referenceType, resolvedRef));
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
        private static object DehydrateOrRecurse([NotNull] this PropertyInfo prop, [NotNull] Type referenceType, [NotNull] object obj)
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
        /// <seealso cref="RepositoryExtensions.Resolve"/>
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
        public static object ResolveReferences([NotNull] this object obj, [NotNull] Type type,
            [NotNull] IReadRepositoryFactory repositoryFactory)
        {
            var newObj = Activator.CreateInstance(type);
            foreach (var prop in type.GetWritableProperties())
            {
                var propertyValue = prop.GetValue(obj, null);
                if (propertyValue == null) continue;

                if (prop.IsCollection())
                {
                    var referenceType = prop.GetGenericArg();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    var resolvedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(newObj, resolvedRefs, null);

                    foreach (object dehydratedRef in (IEnumerable)propertyValue)
                    {
                        if (dehydratedRef == null) continue;

                        collectionType.InvokeAdd(
                            target: resolvedRefs,
                            value: prop.ResolveOrRecurse(referenceType, dehydratedRef, repositoryFactory));
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
            if (prop.HasAttribute<ResolveAttribute>()) return repositoryFactory.Create(referenceType).Resolve((IEntity)obj);
            else if (prop.HasAttribute<ResolveReferencesAttribute>()) return obj.ResolveReferences(referenceType, repositoryFactory);
            return obj;
        }

#if NET45
        /// <summary>
        /// Resolves references that were dehydrated by <see cref="DehydrateReferences{TEntity}"/> to the original full entities. Returns the result as a new object keeping the original unchanged.
        /// </summary>
        /// <param name="obj">The object containing the references to resolve.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        /// <typeparam name="T">The specific type of the <paramref name="obj"/>.</typeparam>
        /// <seealso cref="RepositoryExtensions.Resolve"/>
        [Pure, NotNull]
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
        public static async Task<object> ResolveReferencesAsync([NotNull] this object obj, [NotNull] Type type,
            [NotNull] IReadRepositoryFactory repositoryFactory)
        {
            var newObj = Activator.CreateInstance(type);
            foreach (var prop in type.GetWritableProperties())
            {
                var propertyValue = prop.GetValue(obj, null);
                if (propertyValue == null) continue;

                if (prop.IsCollection())
                {
                    var referenceType = prop.GetGenericArg();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    var resolvedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(newObj, resolvedRefs, null);

                    foreach (object dehydratedRef in (IEnumerable)propertyValue)
                    {
                        if (dehydratedRef == null) continue;

                        collectionType.InvokeAdd(
                            target: resolvedRefs,
                            value: await prop.ResolveOrRecurseAsync(referenceType, dehydratedRef, repositoryFactory));
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

        [NotNull]
        private static async Task<object> ResolveOrRecurseAsync([NotNull] this PropertyInfo prop, [NotNull] Type referenceType,
            [NotNull] object obj, [NotNull] IReadRepositoryFactory repositoryFactory)
        {
            if (prop.HasAttribute<ResolveAttribute>()) return await repositoryFactory.Create(referenceType).ResolveAsync((IEntity)obj);
            else if (prop.HasAttribute<ResolveReferencesAttribute>()) return await obj.ResolveReferencesAsync(referenceType, repositoryFactory);
            return obj;
        }
#endif
    }
}