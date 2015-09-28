using System;

namespace Dehydrator
{
    /// <summary>
    /// Marks a property as a reference or collection of references to be resolved by <see cref="EntityExtensions.ResolveReferences{TEntity}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ResolveAttribute : Attribute
    {
    }
}