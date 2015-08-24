namespace Dehydrator
{
    /// <summary>
    /// An entity (i.e. an object persistable by an ORM).
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The primary key used to store the object in a database.
        /// </summary>
        int Id { get; set; }
    }
}
