using NUnit.Framework;

namespace Dehydrator
{
    [TestFixture]
    public class NoIdTest : EntityExtensionsTest<MockEntity2>
    {
        [SetUp]
        public void SetUp()
        {
            DehydratedRef = new MockEntity2 {Id = Entity.NoId, FriendlyName = "Bar"};
            ResolvedRef = new MockEntity2 {Id = Entity.NoId, FriendlyName = "Bar"};

            EntityWithDehydratedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                SingleRef = DehydratedRef
            };
            EntityWithResolvedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                SingleRef = ResolvedRef
            };
        }
    }
}