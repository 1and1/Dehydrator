using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace EntityReferenceStripper
{
    [PublicAPI]
    public static class EntityExtensions
    {
        /// <summary>
        /// Resolves references that were stripped to contain nothing but their <see cref="IEntity.Id"/>s to the original full entities and returns the result as a new object.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="resolver">Used to aquire full entities based on their ID. Usually backed by a database.</param>
        /// <seealso cref="IEntityResolver.Resolve"/>
        [Pure, NotNull]
        public static T ResolveReferences<T>([NotNull] this T entity, [NotNull] IEntityResolver resolver)
            where T : class, IEntity, new()
        {
            var clonedEntity = entity.CloneMemberwise();

            foreach (var prop in entity.GetVirtualProperties())
            {
                var propertyValue = prop.GetValue(entity, null);
                if (propertyValue == null) continue;

                if (IsEntity(prop))
                {
                    var resolvedReference = resolver.Resolve((IEntity)propertyValue, prop.PropertyType);
                    prop.SetValue(clonedEntity, resolvedReference, null);
                }
                else if (IsEntityCollection(prop))
                {
                    var referenceType = prop.PropertyType.GetGenericArguments().First();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    var resolvedReferences = Activator.CreateInstance(collectionType);
                    prop.SetValue(clonedEntity, resolvedReferences, null);

                    foreach (IEntity strippedReference in (IEnumerable)propertyValue)
                    {
                        if (strippedReference == null) continue;

                        var resolvedReference = resolver.Resolve(strippedReference, referenceType);
                        collectionType.InvokeMember("Add",
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null,
                            resolvedReferences, new object[] { resolvedReference });
                    }
                }
            }

            return clonedEntity;
        }

        /// <summary>
        /// Strips all references to contain nothing but their <see cref="IEntity.Id"/>s and returns the result as a new object.
        /// </summary>
        /// <param name="entity">The entity to strip.</param>
        [Pure, NotNull]
        public static T StripReferences<T>([NotNull] this T entity) where T : class, IEntity, new()
        {
            var clonedEntity = entity.CloneMemberwise();

            foreach (var prop in entity.GetVirtualProperties())
            {
                var propertyValue = prop.GetValue(entity, null);
                if (propertyValue == null) continue;

                if (IsEntity(prop))
                {
                    var strippedReference = (IEntity)Activator.CreateInstance(prop.PropertyType);
                    strippedReference.Id = ((IEntity)propertyValue).Id;
                    prop.SetValue(clonedEntity, strippedReference, null);
                }
                else if (IsEntityCollection(prop))
                {
                    var referenceType = prop.PropertyType.GetGenericArguments().First();
                    var collectionType = typeof(List<>).MakeGenericType(referenceType);

                    var strippedReferences = Activator.CreateInstance(collectionType);
                    prop.SetValue(clonedEntity, strippedReferences, null);

                    foreach (IEntity resolvedReference in (IEnumerable)propertyValue)
                    {
                        if (resolvedReference == null) continue;

                        var strippedReference = (IEntity)Activator.CreateInstance(referenceType);
                        strippedReference.Id = resolvedReference.Id;
                        collectionType.InvokeMember("Add",
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null,
                            strippedReferences, new object[] { strippedReference });
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