using System.Collections.Generic;
using System.Linq;
using FluentAssert;
using Moq;
using NUnit.Framework;

namespace EntityReferenceStripper
{
    public abstract class EntityReferenceTests<TRef>
        where TRef : class, IEntity, new()
    {
        protected MockEntity1 EntityWithStrippedRefs, EntityWithResolvedRefs;
        protected TRef ResolvedRef, StrippedRef;

        [Test]
        public void Resolve()
        {
            var entityResolverMock = new Mock<IEntityResolver>(MockBehavior.Strict);
            entityResolverMock.Setup(x => x.Resolve(StrippedRef, typeof(TRef))).Returns(ResolvedRef);

            var result = EntityWithStrippedRefs.ResolveReferences(entityResolverMock.Object);
            result.ShouldBeEqualTo(EntityWithResolvedRefs);

            entityResolverMock.Verify();
        }

        [Test]
        public void Strip()
        {
            var result = EntityWithResolvedRefs.StripReferences();
            result.ShouldBeEqualTo(EntityWithStrippedRefs);
        }
    }

    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class MockEntity1 : Entity
    {
        public string FriendlyName { get; set; }
        public virtual MockEntity2 SingleRef { get; set; }
        public virtual ICollection<MockEntity2> MultiRef { get; set; } = new List<MockEntity2>();
        public virtual MockEntity1 SingleSelfRef { get; set; }
        public virtual ICollection<MockEntity1> MultiSelfRef { get; set; } = new List<MockEntity1>();
        public MockEntity2 SingleNonRef { get; set; }

        #region Equality

        protected bool Equals(MockEntity1 other)
        {
            return base.Equals(other) && string.Equals(FriendlyName, other.FriendlyName) &&
                   Equals(SingleRef, other.SingleRef) && MultiRef.SequenceEqual(other.MultiRef) &&
                   Equals(SingleSelfRef, other.SingleSelfRef) && MultiSelfRef.SequenceEqual(other.MultiSelfRef) &&
                   Equals(SingleNonRef, other.SingleNonRef);
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
                hashCode = (hashCode * 397) ^ (MultiRef?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (SingleSelfRef?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (MultiSelfRef?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (SingleNonRef?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        #endregion
    }

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