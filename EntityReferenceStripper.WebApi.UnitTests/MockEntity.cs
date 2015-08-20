namespace EntityReferenceStripper.WebApi
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class MockEntity : Entity
    {
        public virtual MockEntity Reference { get; set; }
        public bool Resolved { get; set; }

        #region Equality

        protected bool Equals(MockEntity other)
        {
            return base.Equals(other) && Equals(Reference, other.Reference) && Resolved == other.Resolved;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MockEntity)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Reference?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Resolved.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}