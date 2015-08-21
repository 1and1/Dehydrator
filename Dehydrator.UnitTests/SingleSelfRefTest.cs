using NUnit.Framework;

namespace Dehydrator
{
    [TestFixture]
    public class SingleSelfRefTest : EntityReferenceTests<MockEntity1>
    {
        [SetUp]
        public void SetUp()
        {
            StrippedRef = new MockEntity1 {Id = 2};
            EntityWithStrippedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                SingleSelfRef = StrippedRef
            };

            ResolvedRef = new MockEntity1 {Id = 2, FriendlyName = "Bar"};
            EntityWithResolvedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                SingleSelfRef = ResolvedRef
            };
        }
    }
}
