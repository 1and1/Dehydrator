using System;

namespace Dehydrator
{
    /// <summary>
    /// Marks a property as containing an entity (composition). This entity is to be passed to <see cref="EntityExtensions.DehydrateReferences{TEntity}"/>. Automatically implies <see cref="ResolveReferencesAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DehydrateReferencesAttribute : ResolveReferencesAttribute
    {
    }
}
