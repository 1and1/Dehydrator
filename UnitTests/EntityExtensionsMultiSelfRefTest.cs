﻿using NUnit.Framework;

namespace Dehydrator
{
    [TestFixture]
    public class EntityExtensionsMultiSelfRefTest : EntityExtensionsTest<MockEntity1>
    {
        [SetUp]
        public void SetUp()
        {
            DehydratedRef = new MockEntity1 {Id = 2};
            ResolvedRef = new MockEntity1 {Id = 2, FriendlyName = "Bar"};

            EntityWithDehydratedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                MultiSelfRef = {DehydratedRef}
            };
            EntityWithResolvedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                MultiSelfRef = {ResolvedRef}
            };
        }
    }
}