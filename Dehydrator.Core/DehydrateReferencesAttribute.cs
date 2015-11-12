using System;

namespace Dehydrator
{
    /// <summary>
    /// Marks a property as containing an entity or collection of entities (composition).Automatically implies <see cref="ResolveReferencesAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DehydrateReferencesAttribute : ResolveReferencesAttribute
    {
    }
}
