using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace Dehydrator.EntityFramework.Unity
{
    [TestFixture]
    public class UnityContainerExtensionsTest
    {
        [Test]
        public void Test()
        {
            var container = new UnityContainer();
            container.RegisterDatabase<MockDbContext>()
                .RegisterRepository(x => x.MockEntity1s)
                .RegisterRepository(x => x.MockEntity2s);

            var resolve1 = container.CreateChildContainer().Resolve<ResolveTree>();
            Assert.AreSame(resolve1.DbContext, resolve1.Repo1.DbContext);
            Assert.AreSame(resolve1.DbContext, resolve1.Repo2.DbContext);

            var resolve2 = container.CreateChildContainer().Resolve<ResolveTree>();
            Assert.AreNotSame(resolve1.DbContext, resolve2.DbContext);
        }

        private class ResolveTree
        {
            public MockDbContext DbContext { get; }
            public DbCrudRepository<MockEntity1> Repo1 { get; }
            public DbCrudRepository<MockEntity2> Repo2 { get; }

            public ResolveTree(MockDbContext dbContext, ICrudRepository<MockEntity1> repo1, ICrudRepository<MockEntity2> repo2)
            {
                DbContext = dbContext;
                Repo1 = (DbCrudRepository<MockEntity1>)repo1;
                Repo2 = (DbCrudRepository<MockEntity2>)repo2;
            }
        }
    }
}
