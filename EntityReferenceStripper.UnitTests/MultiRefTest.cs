using NUnit.Framework;

namespace EntityReferenceStripper
{
    [TestFixture]
    public class MultiRefTest : EntityReferenceTests<MockEntity2>
    {
        [SetUp]
        public void SetUp()
        {
            StrippedRef = new MockEntity2 {Id = 2};
            EntityWithStrippedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                MultiRef = {StrippedRef}
            };

            ResolvedRef = new MockEntity2 {Id = 2, FriendlyName = "Bar"};
            EntityWithResolvedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                MultiRef = {ResolvedRef}
            };
        }
    }
}