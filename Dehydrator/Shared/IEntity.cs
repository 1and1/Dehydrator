using System.ComponentModel;

namespace Dehydrator
{
    /// <summary>
    /// An entity (i.e. an object persistable by an ORM).
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The primary key used to store the object in a database. Use <see cref="Entity.NoId"/> to indicate that no ID has been assigned yet (and therefore cannot be dehydrated).
        /// </summary>
        [DefaultValue(Entity.NoId)]
        long Id { get; set; }
    }
}
