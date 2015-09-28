using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Provides extension methods for <see cref="IEnumerable{T}"/>s.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Filters a sequence of elements to remove any <see langword="null"/> values.
        /// </summary>
        [NotNull, ItemNotNull, Pure, LinqTunnel]
        public static IEnumerable<T> WhereNotNull<T>([NotNull, ItemCanBeNull] this IEnumerable<T> enumeration)
        {
            return enumeration.Where(element => element != null);
        }

        /// <summary>
        /// Determines whether two collections contain the same elements disregarding the order they are in.
        /// </summary>
        /// <param name="first">The first of the two collections to compare.</param>
        /// <param name="second">The first of the two collections to compare.</param>
        /// <param name="comparer">Controls how to compare elements; leave <see langword="null"/> for default comparer.</param>
        [Pure]
        public static bool UnsequencedEquals<T>([NotNull, InstantHandle] this ICollection<T> first,
            [NotNull, InstantHandle] ICollection<T> second, [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            if (first.Count != second.Count) return false;
            if (comparer == null) comparer = EqualityComparer<T>.Default;

            if (first.GetUnsequencedHashCode(comparer) != second.GetUnsequencedHashCode(comparer)) return false;
            return first.All(x => second.Contains(x, comparer));
        }

        /// <summary>
        /// Generates a hash code for the contents of a collection. Changing the elements' order will not change the hash.
        /// </summary>
        /// <param name="collection">The collection to generate the hash for.</param>
        /// <param name="comparer">Controls how to compare elements; leave <see langword="null"/> for default comparer.</param>
        /// <seealso cref="UnsequencedEquals{T}"/>
        [Pure]
        public static int GetUnsequencedHashCode<T>([NotNull, InstantHandle] this IEnumerable<T> collection,
            [CanBeNull, InstantHandle] IEqualityComparer<T> comparer = null)
        {
            if (comparer == null) comparer = EqualityComparer<T>.Default;

            unchecked
            {
                int result = 397;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (T unknown in collection.WhereNotNull())
                    result = result ^ comparer.GetHashCode(unknown);
                return result;
            }
        }

        /// <summary>
        /// Determines the element in a list that maximizes a specified expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <typeparam name="TValue">The type of the <paramref name="expression"/>.</typeparam>
        /// <param name="enumeration">The elements to check.</param>
        /// <param name="expression">The expression to maximize.</param>
        /// <returns>The element that maximizes the expression; the default value of <typeparamref name="T"/> if <paramref name="enumeration"/> contains no elements.</returns>
        [CanBeNull, Pure, LinqTunnel]
        public static T MaxBy<T, TValue>([NotNull, ItemNotNull] this IEnumerable<T> enumeration,
            [NotNull, InstantHandle] Func<T, TValue> expression)
        {
            return enumeration.MaxBy(expression, Comparer<TValue>.Default);
        }

        /// <summary>
        /// Determines the element in a list that maximizes a specified expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <typeparam name="TValue">The type of the <paramref name="expression"/>.</typeparam>
        /// <param name="enumeration">The elements to check.</param>
        /// <param name="expression">The expression to maximize.</param>
        /// <param name="comparer">A comprarer used to compare values of <paramref name="expression"/>.</param>
        /// <returns>The element that maximizes the expression; the default value of <typeparamref name="T"/> if <paramref name="enumeration"/> contains no elements.</returns>
        [CanBeNull, Pure, LinqTunnel]
        public static T MaxBy<T, TValue>([NotNull, ItemNotNull] this IEnumerable<T> enumeration,
            [NotNull, InstantHandle] Func<T, TValue> expression, [NotNull] IComparer<TValue> comparer)
        {
            using (var enumerator = enumeration.GetEnumerator())
            {
                if (!enumerator.MoveNext()) return default(T);
                T maxElement = enumerator.Current;
                TValue maxValue = expression(maxElement);

                while (enumerator.MoveNext())
                {
                    var candidate = enumerator.Current;
                    var candidateValue = expression(candidate);
                    if (comparer.Compare(candidateValue, maxValue) > 0)
                    {
                        maxElement = candidate;
                        maxValue = candidateValue;
                    }
                }

                return maxElement;
            }
        }

        /// <summary>
        /// Determines the element in a list that minimizes a specified expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <typeparam name="TValue">The type of the <paramref name="expression"/>.</typeparam>
        /// <param name="enumeration">The elements to check.</param>
        /// <param name="expression">The expression to minimize.</param>
        /// <returns>The element that minimizes the expression; the default value of <typeparamref name="T"/> if <paramref name="enumeration"/> contains no elements.</returns>
        [CanBeNull, Pure, LinqTunnel]
        public static T MinBy<T, TValue>([NotNull, ItemNotNull] this IEnumerable<T> enumeration,
            [NotNull, InstantHandle] Func<T, TValue> expression)
        {
            return enumeration.MinBy(expression, Comparer<TValue>.Default);
        }

        /// <summary>
        /// Determines the element in a list that minimizes a specified expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <typeparam name="TValue">The type of the <paramref name="expression"/>.</typeparam>
        /// <param name="enumeration">The elements to check.</param>
        /// <param name="expression">The expression to minimize.</param>
        /// <param name="comparer">A comprarer used to compare values of <paramref name="expression"/>.</param>
        /// <returns>The element that minimizes the expression; the default value of <typeparamref name="T"/> if <paramref name="enumeration"/> contains no elements.</returns>
        [CanBeNull, Pure, LinqTunnel]
        public static T MinBy<T, TValue>([NotNull, ItemNotNull] this IEnumerable<T> enumeration,
            [NotNull, InstantHandle] Func<T, TValue> expression, [NotNull] IComparer<TValue> comparer)
        {
            using (var enumerator = enumeration.GetEnumerator())
            {
                if (!enumerator.MoveNext()) return default(T);
                T minElement = enumerator.Current;
                TValue minValue = expression(minElement);

                while (enumerator.MoveNext())
                {
                    var candidate = enumerator.Current;
                    var candidateValue = expression(candidate);
                    if (comparer.Compare(candidateValue, minValue) < 0)
                    {
                        minElement = candidate;
                        minValue = candidateValue;
                    }
                }

                return minElement;
            }
        }

        /// <summary>
        /// Calls <see cref="ICloneable.Clone"/> for every element in a collection and returns the results as a new collection.
        /// </summary>
        [NotNull, Pure]
        public static IEnumerable<T> CloneElements<T>([NotNull, ItemNotNull] this IEnumerable<T> enumerable)
            where T : ICloneable
        {
            return enumerable.Select(entry => (T)entry.Clone());
        }
    }
}