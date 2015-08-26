using System;

namespace Dehydrator
{
    /// <summary>
    /// Marks a property as a reference to be dehydrated by <see cref="EntityExtensions.DehydrateReferences{TEntity}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DehydrateAttribute : Attribute
    {
    }
}
