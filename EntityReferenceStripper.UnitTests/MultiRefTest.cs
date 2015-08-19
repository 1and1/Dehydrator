using NUnit.Framework;

namespace EntityReferenceStripper
{
    [TestFixture]
    public class MultiRefTest : EntityReferenceTests
    {
        [SetUp]
        public void SetUp()
        {
            var strippedRef = new MockEntity2 {Id = 2};
            EntityWithStrippedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                MultiRef = {strippedRef}
            };

            var resolvedRef = new MockEntity2 {Id = 2, FriendlyName = "Bar"};
            EntityWithResolvedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                MultiRef = {resolvedRef}
            };

            EntityResolverMock.Setup(x => x.Resolve(strippedRef, typeof (MockEntity2))).Returns(resolvedRef);
        }
    }
}