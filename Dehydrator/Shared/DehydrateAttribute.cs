using System;

namespace Dehydrator
{
    /// <summary>
    /// Marks a property as a reference or collection of references to be dehydrated by <see cref="EntityExtensions.DehydrateReferences{TEntity}"/>. Automatically implies <see cref="ResolveAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DehydrateAttribute : ResolveAttribute
    {
    }
}
