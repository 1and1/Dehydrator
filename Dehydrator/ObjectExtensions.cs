using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Dehydrator
{
    internal static class ObjectExtensions
    {
        private static readonly MethodInfo MemberwiseCloneMethod = typeof (object).GetMethod(nameof(MemberwiseClone),
            BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Performs a member-wise clone of the object from the outside, bypassing the "protected" restriction of <see cref="object.MemberwiseClone"/>
        /// </summary>
        [Pure, NotNull]
        public static T CloneMemberwise<T>([NotNull] this T obj)
        {
            return (T) MemberwiseCloneMethod.Invoke(obj, null);
        }

        /// <summary>
        /// Returns all virtual properties defined on the type of this object.
        /// </summary>
        [Pure, NotNull]
        public static IEnumerable<PropertyInfo> GetVirtualProperties([NotNull] this object obj)
        {
            return obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.GetGetMethod().IsVirtual);
        }
    }
}
