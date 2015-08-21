using NUnit.Framework;

namespace Dehydrator
{
    [TestFixture]
    public class MultiSelfRefTest : EntityReferenceTests<MockEntity1>
    {
        [SetUp]
        public void SetUp()
        {
            StrippedRef = new MockEntity1 {Id = 2};
            EntityWithStrippedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                MultiSelfRef = {StrippedRef}
            };

            ResolvedRef = new MockEntity1 {Id = 2, FriendlyName = "Bar"};
            EntityWithResolvedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                MultiSelfRef = {ResolvedRef}
            };
        }
    }
}
