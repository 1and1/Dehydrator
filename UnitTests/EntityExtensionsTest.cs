using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Dehydrator
{
    public abstract class EntityExtensionsTest<TRef>
        where TRef : class, IEntity, new()
    {
        protected MockEntity1 EntityWithDehydratedRefs, EntityWithResolvedRefs;
        protected TRef ResolvedRef, DehydratedRef;

        [Test]
        public void Resolve()
        {
            var repositoryMock = new Mock<ICrudRepository<TRef>>(MockBehavior.Strict);
            if (DehydratedRef.Id != Entity.NoId)
            {
                repositoryMock.Setup(x => x.Find(DehydratedRef.Id))
                    .Returns(ResolvedRef);
            }

            var factoryMock = new Mock<IReadRepositoryFactory>(MockBehavior.Loose);
            factoryMock.Setup(x => x.Create<TRef>())
                .Returns(repositoryMock.Object);

            var result = new MockContainer(EntityWithDehydratedRefs).ResolveReferences(factoryMock.Object);
            result.Entities.First()
                .Should().Be(EntityWithResolvedRefs);

            factoryMock.VerifyAll();
            repositoryMock.VerifyAll();
        }

        [Test]
        public async Task ResolveAsync()
        {
            var repositoryMock = new Mock<ICrudRepository<TRef>>(MockBehavior.Strict);
            if (DehydratedRef.Id != Entity.NoId)
            {
                repositoryMock.Setup(x => x.FindAsync(DehydratedRef.Id))
                    .ReturnsAsync(ResolvedRef);
            }

            var factoryMock = new Mock<IReadRepositoryFactory>(MockBehavior.Loose);
            factoryMock.Setup(x => x.Create<TRef>())
                .Returns(repositoryMock.Object);

            var result = await new MockContainer(EntityWithDehydratedRefs).ResolveReferencesAsync(factoryMock.Object);
            result.Entities.First()
                .Should().Be(EntityWithResolvedRefs);

            factoryMock.VerifyAll();
            repositoryMock.VerifyAll();
        }

        [Test]
        public virtual void Dehydrate()
        {
            var result = new MockContainer(EntityWithResolvedRefs).DehydrateReferences();
            result.Entities.First()
                .Should().Be(EntityWithDehydratedRefs);
        }
    }
}