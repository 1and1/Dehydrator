using System;

namespace Dehydrator
{
    /// <summary>
    /// Marks a property as containing an entity or collection of entities (composition).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ResolveReferencesAttribute : Attribute
    {
    }
}