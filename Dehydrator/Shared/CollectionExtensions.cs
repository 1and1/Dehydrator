﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Dehydrator
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Removes all items from a <paramref name="collection"/> that match a specific <paramref name="condition"/>.
        /// </summary>
        /// <returns><see langword="true"/> if any elements where removed.</returns>
        /// <seealso cref="List{T}.RemoveAll"/>
        public static bool RemoveAll<T>([NotNull, InstantHandle] this ICollection<T> collection,
            [NotNull] Func<T, bool> condition)
        {
            bool removedAny = false;
            foreach (var item in collection.Where(condition).ToList())
            {
                collection.Remove(item);
                removedAny = true;
            }
            return removedAny;
        }

        /// <summary>
        /// Finds the index of a specific <paramref name="element"/> in a <paramref name="collection"/>.
        /// </summary>
        public static int FindIndex<T>([NotNull, InstantHandle] this List<T> collection, [CanBeNull] T element)
        {
            return collection.FindIndex(x => Equals(x, element));
        }
    }
}