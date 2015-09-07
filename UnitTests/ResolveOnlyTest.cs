using FluentAssertions;
using NUnit.Framework;

namespace Dehydrator
{
    [TestFixture]
    public class ResolveOnlyTest : EntityExtensionsTest<MockEntity2>
    {
        [SetUp]
        public void SetUp()
        {
            DehydratedRef = new MockEntity2 {Id = 2};
            ResolvedRef = new MockEntity2 {Id = 2, FriendlyName = "Bar"};

            EntityWithDehydratedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                ResolveOnly = DehydratedRef
            };
            EntityWithResolvedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                ResolveOnly = ResolvedRef
            };
        }

        [Test]
        public override void Dehydrate()
        {
            var result = EntityWithResolvedRefs.DehydrateReferences();
            result.Should().Be(EntityWithResolvedRefs);
        }
    }
}