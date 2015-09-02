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
            var repositoryMock = new Mock<IRepository<TRef>>(MockBehavior.Strict);
            if (DehydratedRef.Id != Entity.NoId)
            {
                repositoryMock.Setup(x => x.Find(DehydratedRef.Id))
                    .Returns(ResolvedRef).Verifiable();
            }

            var factoryMock = new Mock<IRepositoryFactory>(MockBehavior.Loose);
            factoryMock.Setup(x => x.Create<TRef>())
                .Returns(repositoryMock.Object).Verifiable();

            var result = EntityWithDehydratedRefs.ResolveReferences(factoryMock.Object);
            result.Should().Be(EntityWithResolvedRefs);

            factoryMock.Verify();
            repositoryMock.Verify();
        }

        [Test]
        public async void ResolveAsync()
        {
            var repositoryMock = new Mock<IRepository<TRef>>(MockBehavior.Strict);
            if (DehydratedRef.Id != Entity.NoId)
            {
                repositoryMock.Setup(x => x.FindUntypedAsync(DehydratedRef.Id))
                    .Returns(Task.FromResult((IEntity)ResolvedRef)).Verifiable();
            }

            var factoryMock = new Mock<IRepositoryFactory>(MockBehavior.Loose);
            factoryMock.Setup(x => x.Create<TRef>())
                .Returns(repositoryMock.Object).Verifiable();

            var result = await EntityWithDehydratedRefs.ResolveReferencesAsync(factoryMock.Object);
            result.Should().Be(EntityWithResolvedRefs);

            factoryMock.Verify();
            repositoryMock.Verify();
        }

        [Test]
        public virtual void Dehydrate()
        {
            var result = EntityWithResolvedRefs.DehydrateReferences();
            result.Should().Be(EntityWithDehydratedRefs);
        }
    }
}