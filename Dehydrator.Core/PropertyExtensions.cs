using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Provides extension methods for reflection on properties.
    /// </summary>
    public static class PropertyExtensions
    {
        /// <summary>
        /// Determines whether the given <paramref name="type"/> is a reference type and has a default (parameterless) constructor.
        /// </summary>
        public static bool IsPoco([NotNull] this Type type)
        {
            return !type.IsInterface && !type.IsValueType && (type.GetConstructor(new Type[] {}) != null);
        }

        /// <summary>
        /// Returns all properties with public setters and getters defined on a type.
        /// </summary>
        [NotNull]
        public static IEnumerable<PropertyInfo> GetWritableProperties([NotNull] this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetGetMethod() != null && x.GetSetMethod() != null);
        }

        /// <summary>
        /// Returns the first generic argument of the property's type.
        /// </summary>
        [NotNull]
        public static Type GetGenericArg([NotNull] this PropertyInfo prop)
        {
            return prop.PropertyType.GetGenericArguments().First();
        }

        /// <summary>
        /// Determines whether a property is marked with <typeparamref name="TAttribute"/>.
        /// </summary>
        public static bool HasAttribute<TAttribute>([NotNull] this PropertyInfo prop)
            where TAttribute : Attribute
        {
            return prop.GetCustomAttributes(typeof(TAttribute), inherit: true).Any();
        }

        /// <summary>
        /// Determines whether a property holds an <see cref="ICollection{T}"/>.
        /// </summary>
        public static bool IsCollection([NotNull] this PropertyInfo prop)
        {
            return
                (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)) ||
                prop.PropertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));
        }
    }
}
