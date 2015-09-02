using System;

namespace Dehydrator
{
    /// <summary>
    /// Marks a property as containing an entity (composition). This entity is to be passed to <see cref="EntityExtensions.ResolveReferences{TEntity}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ResolveReferencesAttribute : Attribute
    {
    }
}