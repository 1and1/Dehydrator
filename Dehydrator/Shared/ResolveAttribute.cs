using System;

namespace Dehydrator
{
    /// <summary>
    /// Marks a property as a reference to be resolved when this entity is passed to <see cref="EntityExtensions.ResolveReferences{TEntity}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ResolveAttribute : Attribute
    {
    }
}