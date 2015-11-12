namespace Dehydrator
{
    /// <summary>
    /// An entity with a name.
    /// </summary>
    public interface INamedEntity : IEntity
    {
        /// <summary>
        /// A (human-readable) name for the entity. Not used as an ID. Is preserved on dehydration.
        /// </summary>
        string Name { get; set; }
    }
}
