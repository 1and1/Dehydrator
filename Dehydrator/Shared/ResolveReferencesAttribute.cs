using System;

namespace Dehydrator
{
    /// <summary>
    /// Marks a property as containing an entity or collection of entities (composition). <see cref="EntityExtensions.ResolveReferences{TEntity}"/>  recurses into this.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ResolveReferencesAttribute : Attribute
    {
    }
}