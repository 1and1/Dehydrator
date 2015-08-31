using NUnit.Framework;

namespace Dehydrator
{
    [TestFixture]
    public class EntityExtensionsSingleRefTest : EntityExtensionsTest<MockEntity2>
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