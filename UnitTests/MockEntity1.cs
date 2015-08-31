using System.Collections.Generic;
using System.Linq;

namespace Dehydrator
{
    public class MockEntity1 : Entity
    {
        public string FriendlyName { get; set; }

        [Dehydrate]
        public virtual MockEntity2 SingleRef { get; set; }

        [Dehydrate]
        public virtual ICollection<MockEntity2> MultiRef { get; set; } = new List<MockEntity2>();

        [Dehydrate]
        public virtual MockEntity1 SingleSelfRef { get; set; }

        [Dehydrate]
        public virtual ICollection<MockEntity1> MultiSelfRef { get; set; } = new List<MockEntity1>();

        public MockEntity1 SingleRecurse { get; set; }

        public ICollection<MockEntity1> MultiRecurse { get; set; } = new List<MockEntity1>();

        [Resolve]
        public MockEntity2 ResolveOnly { get; set; }

        #region Equality
        protected bool Equals(MockEntity1 other)
        {
            return base.Equals(other) && string.Equals(FriendlyName, other.FriendlyName) &&
                   Equals(SingleRef, other.SingleRef) && MultiRef.SequenceEqual(other.MultiRef) &&
                   Equals(SingleSelfRef, other.SingleSelfRef) && MultiSelfRef.SequenceEqual(other.MultiSelfRef) &&
                   Equals(SingleRecurse, other.SingleRecurse) && MultiRecurse.SequenceEqual(other.MultiRecurse) &&
                   Equals(ResolveOnly, other.ResolveOnly);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MockEntity1)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (FriendlyName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (SingleRef?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (SingleSelfRef?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (SingleRecurse?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (ResolveOnly?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
        #endregion
    }
}