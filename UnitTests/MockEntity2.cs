namespace Dehydrator
{
    public class MockEntity2 : Entity
    {
        public string FriendlyName { get; set; }

        #region Equality
        protected bool Equals(MockEntity2 other)
        {
            return base.Equals(other) && string.Equals(FriendlyName, other.FriendlyName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MockEntity2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (FriendlyName?.GetHashCode() ?? 0);
            }
        }
        #endregion
    }
}