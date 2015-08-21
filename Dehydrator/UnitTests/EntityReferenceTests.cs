using System.Threading.Tasks;
using FluentAssert;
using Moq;
using NUnit.Framework;

namespace Dehydrator
{
    public abstract class EntityReferenceTests<TRef>
        where TRef : class, IEntity, new()
    {
        protected MockEntity1 EntityWithDehydratedRefs, EntityWithResolvedRefs;
        protected TRef ResolvedRef, DehydratedRef;

        [Test]
        public void Resolve()
        {
            var repositoryMock = new Mock<IEntityRepository<IEntity>>(MockBehavior.Strict);
            repositoryMock.Setup(x => x.Find(DehydratedRef.Id)).Returns(ResolvedRef);

            var factoryMock = new Mock<IEntityRepositoryFactory>(MockBehavior.Loose);
            factoryMock.Setup(x => x.Create(typeof(TRef))).Returns(repositoryMock.Object);

            var result = EntityWithDehydratedRefs.ResolveReferences(factoryMock.Object);
            result.ShouldBeEqualTo(EntityWithResolvedRefs);

            factoryMock.Verify();
            repositoryMock.Verify();
        }

        [Test]
        public async void ResolveAsync()
        {
            var repositoryMock = new Mock<IEntityRepository<IEntity>>(MockBehavior.Strict);
            repositoryMock.Setup(x => x.FindAsync(DehydratedRef.Id)).Returns(Task.FromResult((IEntity)ResolvedRef));

            var factoryMock = new Mock<IEntityRepositoryFactory>(MockBehavior.Loose);
            factoryMock.Setup(x => x.Create(typeof(TRef))).Returns(repositoryMock.Object);

            var result = await EntityWithDehydratedRefs.ResolveReferencesAsync(factoryMock.Object);
            result.ShouldBeEqualTo(EntityWithResolvedRefs);

            factoryMock.Verify();
            repositoryMock.Verify();
        }

        [Test]
        public void Dehydrate()
        {
            var result = EntityWithResolvedRefs.DehydrateReferences();
            result.ShouldBeEqualTo(EntityWithDehydratedRefs);
        }
    }
}
