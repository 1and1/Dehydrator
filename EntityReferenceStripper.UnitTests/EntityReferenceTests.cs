using System.Collections.Generic;
using System.Linq;
using FluentAssert;
using Moq;
using NUnit.Framework;

namespace EntityReferenceStripper
{
    public abstract class EntityReferenceTests
    {
        protected MockEntity1 EntityWithStrippedRefs;
        protected MockEntity1 EntityWithResolvedRefs;

        protected readonly Mock<IEntityResolver> EntityResolverMock = new Mock<IEntityResolver>(MockBehavior.Strict);

        [TearDown]
        public void TearDown()
        {
            EntityResolverMock.Verify();
        }

        [Test]
        public void Resolve()
        {
            var result = EntityWithStrippedRefs.ResolveReferences(EntityResolverMock.Object);
            result.ShouldBeEqualTo(EntityWithResolvedRefs);
        }

        [Test]
        public void Strip()
        {
            var result = EntityWithResolvedRefs.StripReferences();
            result.ShouldBeEqualTo(EntityWithStrippedRefs);
        }

        // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
        protected class MockEntity1 : Entity
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
                return Equals((MockEntity1) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = base.GetHashCode();
                    hashCode = (hashCode*397) ^ (FriendlyName?.GetHashCode() ?? 0);
                    hashCode = (hashCode*397) ^ (SingleRef?.GetHashCode() ?? 0);
                    hashCode = (hashCode*397) ^ (MultiRef?.GetHashCode() ?? 0);
                    hashCode = (hashCode*397) ^ (SingleSelfRef?.GetHashCode() ?? 0);
                    hashCode = (hashCode*397) ^ (MultiSelfRef?.GetHashCode() ?? 0);
                    hashCode = (hashCode*397) ^ (SingleNonRef?.GetHashCode() ?? 0);
                    return hashCode;
                }
            }

            #endregion
        }

        protected class MockEntity2 : Entity
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
                return Equals((MockEntity2) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (base.GetHashCode()*397) ^ (FriendlyName?.GetHashCode() ?? 0);
                }
            }

            #endregion
        }
    }
}