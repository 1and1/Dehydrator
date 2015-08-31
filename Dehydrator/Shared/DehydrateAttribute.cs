using System;

namespace Dehydrator
{
    /// <summary>
    /// Marks a property as a reference to be dehydrated by <see cref="EntityExtensions.DehydrateReferences{TEntity}"/>. Automatically implies <see cref="ResolveAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DehydrateAttribute : ResolveAttribute
    {
    }
}
