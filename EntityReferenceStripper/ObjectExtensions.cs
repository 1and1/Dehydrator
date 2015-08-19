using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace EntityReferenceStripper
{
    internal static class ObjectExtensions
    {
        private static readonly MethodInfo _memberwiseCloneMethod = typeof (object).GetMethod(nameof(MemberwiseClone),
            BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Performs a member-wise clone of the object from the outside, bypassing the "protected" restriction of <see cref="object.MemberwiseClone"/>
        /// </summary>
        [NotNull]
        public static T CloneMemberwise<T>([NotNull] this T obj)
        {
            return (T) _memberwiseCloneMethod.Invoke(obj, null);
        }

        /// <summary>
        /// Returns all virtual properties defined on the type of this object.
        /// </summary>
        [NotNull]
        public static IEnumerable<PropertyInfo> GetVirtualProperties([NotNull] this object obj)
        {
            return obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.GetGetMethod().IsVirtual);
        }
    }
}