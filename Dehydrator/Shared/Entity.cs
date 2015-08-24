using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// A base implementation of <see cref="IEntity"/>. You can derive from this or implement <see cref="IEntity"/> directly.
    /// </summary>
    [PublicAPI]
    public abstract class Entity : IEntity
    {
        public long Id { get; set; }

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
        #endregion
    }
}
