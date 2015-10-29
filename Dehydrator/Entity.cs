using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// A base implementation of <see cref="IEntity"/>. You can derive from this or implement <see cref="IEntity"/> directly.
    /// </summary>
    public abstract class Entity : IEntity
    {
        [Key, DefaultValue(NoId)]
        public virtual long Id { get; set; } = NoId;

        /// <summary>
        /// Value for <see cref="IEntity.Id"/> to indicate that no ID has been assigned yet.
        /// </summary>
        public const long NoId = 0;

        public override string ToString()
        {
            return GetType().Name + " " + Id;
        }

        #region Equality
        protected bool Equals(Entity other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Entity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Compares two <see cref="IEntity"/> collections for element-wise equality. Treats <see langword="null"/> values as empty collections.
        /// </summary>
        protected bool Equals<T>([CanBeNull] ICollection<T> first, [CanBeNull] ICollection<T> second)
        {
            return (first ?? new List<T>()).UnsequencedEquals(second ?? new List<T>());
        }
        #endregion
    }
}
