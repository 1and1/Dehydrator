using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace EntityReferenceStripper
{
    [PublicAPI]
    public static class EntityExtensions
    {
        /// <summary>
        /// Resolves references that were stripped to contain nothing but their <see cref="IEntity.Id"/>s to the original full entities and returns the result as a new object.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        /// <seealso cref="EntitryRepositoryExtensions.Resolve{TEntity}"/>
        [Pure, NotNull]
        public static TEntity ResolveReferences<TEntity>([NotNull] this TEntity entity,
            [NotNull] IEntityRepositoryFactory repositoryFactory)
            where TEntity : class, IEntity
        {
            var clonedEntity = entity.CloneMemberwise();

            foreach (var prop in entity.GetVirtualProperties())
            {
                var propertyValue = prop.GetValue(entity, null);
                if (propertyValue == null) continue;

                if (IsEntity(prop))
                {
                    var referenceFactory = repositoryFactory.Create(prop.PropertyType);
                    var resolvedRef = referenceFactory.Resolve((IEntity)propertyValue);
                    prop.SetValue(clonedEntity, resolvedRef, null);
                }
                else if (IsEntityCollection(prop))
                {
                    var referenceType = prop.PropertyType.GetGenericArguments().First();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    var resolvedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(clonedEntity, resolvedRefs, null);

                    var referenceRepository = repositoryFactory.Create(referenceType);
                    foreach (IEntity strippedRef in (IEnumerable)propertyValue)
                    {
                        if (strippedRef == null) continue;

                        var resolvedRef = referenceRepository.Resolve(strippedRef);
                        collectionType.InvokeMember("Add",
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null,
                            resolvedRefs, new object[] {resolvedRef});
                    }
                }
            }

            return clonedEntity;
        }

#if NET45
        /// <summary>
        /// Resolves references that were stripped to contain nothing but their <see cref="IEntity.Id"/>s to the original full entities and returns the result as a new object.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="repositoryFactory">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        /// <seealso cref="EntitryRepositoryExtensions.ResolveAsync{TEntity}"/>
        [Pure, NotNull]
        public static async Task<TEntity> ResolveReferencesAsync<TEntity>([NotNull] this TEntity entity,
            [NotNull] IEntityRepositoryFactory repositoryFactory)
            where TEntity : class, IEntity
        {
            var clonedEntity = entity.CloneMemberwise();

            foreach (var prop in entity.GetVirtualProperties())
            {
                var propertyValue = prop.GetValue(entity, null);
                if (propertyValue == null) continue;

                if (IsEntity(prop))
                {
                    var referenceFactory = repositoryFactory.Create(prop.PropertyType);
                    var resolvedRef = await referenceFactory.ResolveAsync((IEntity)propertyValue);
                    prop.SetValue(clonedEntity, resolvedRef, null);
                }
                else if (IsEntityCollection(prop))
                {
                    var referenceType = prop.PropertyType.GetGenericArguments().First();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    var resolvedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(clonedEntity, resolvedRefs, null);

                    var referenceRepository = repositoryFactory.Create(referenceType);
                    foreach (IEntity strippedRef in (IEnumerable)propertyValue)
                    {
                        if (strippedRef == null) continue;

                        var resolvedRef = await referenceRepository.ResolveAsync(strippedRef);
                        collectionType.InvokeMember("Add",
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null,
                            resolvedRefs, new object[] {resolvedRef});
                    }
                }
            }

            return clonedEntity;
        }
#endif

        /// <summary>
        /// Strips all references to contain nothing but their <see cref="IEntity.Id"/>s and returns the result as a new object.
        /// </summary>
        /// <param name="entity">The entity to strip.</param>
        [Pure, NotNull]
        public static TEntity StripReferences<TEntity>([NotNull] this TEntity entity)
            where TEntity : class, IEntity
        {
            var clonedEntity = entity.CloneMemberwise();

            foreach (var prop in entity.GetVirtualProperties())
            {
                var propertyValue = prop.GetValue(entity, null);
                if (propertyValue == null) continue;

                if (IsEntity(prop))
                {
                    var strippedRef = (IEntity)Activator.CreateInstance(prop.PropertyType);
                    strippedRef.Id = ((IEntity)propertyValue).Id;
                    prop.SetValue(clonedEntity, strippedRef, null);
                }
                else if (IsEntityCollection(prop))
                {
                    var referenceType = prop.PropertyType.GetGenericArguments().First();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    var strippedRefs = Activator.CreateInstance(collectionType);
                    prop.SetValue(clonedEntity, strippedRefs, null);

                    foreach (IEntity resolvedRef in (IEnumerable)propertyValue)
                    {
                        if (resolvedRef == null) continue;

                        var strippedRef = (IEntity)Activator.CreateInstance(referenceType);
                        strippedRef.Id = resolvedRef.Id;
                        collectionType.InvokeMember("Add",
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null,
                            strippedRefs, new object[] {strippedRef});
                    }
                }
            }

            return clonedEntity;
        }

        private static bool IsEntity([NotNull] PropertyInfo prop)
        {
            return typeof(IEntity).IsAssignableFrom(prop.PropertyType);
        }

        private static bool IsEntityCollection([NotNull] PropertyInfo prop)
        {
            return prop.PropertyType.IsGenericType &&
                   prop.PropertyType.GetGenericTypeDefinition() ==
                   typeof(ICollection<>) &&
                   typeof(IEntity).IsAssignableFrom(prop.PropertyType.GetGenericArguments().First());
        }
    }
}