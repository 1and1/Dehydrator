using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Provides extension methods for <see cref="IEntity"/>s.
    /// </summary>
    [PublicAPI]
    public static class EntityExtensions
    {

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
                    if (targetList == null)
                    {
                        var collectionType = typeof(List<>).MakeGenericType(referenceType);
                        targetList = Activator.CreateInstance(collectionType);
                        prop.SetValue(obj: to, value: targetList, index: null);
                    }
                    else ((dynamic)targetList).Clear();

                    dynamic targetListDynamic = targetList;
                    foreach (var reference in (IEnumerable)fromValue)
                        targetListDynamic.Add((dynamic)reference);
                }
                else prop.SetValue(obj: to, value: fromValue, index: null);
            }
        }
    }
}