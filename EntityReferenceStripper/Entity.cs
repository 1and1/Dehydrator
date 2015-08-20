using JetBrains.Annotations;

namespace EntityReferenceStripper
{
    [PublicAPI]
    public abstract class Entity : IEntity
    {
        public int Id { get; set; }

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
            return Id;
        }

        #endregion
    }
}