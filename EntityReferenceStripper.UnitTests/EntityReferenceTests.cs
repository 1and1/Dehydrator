using System.Threading.Tasks;
using FluentAssert;
using Moq;
using NUnit.Framework;

namespace EntityReferenceStripper
{
    public abstract class EntityReferenceTests<TRef>
        where TRef : class, IEntity, new()
    {
        protected MockEntity1 EntityWithStrippedRefs, EntityWithResolvedRefs;
        protected TRef ResolvedRef, StrippedRef;

        [Test]
        public void Resolve()
        {
            var repositoryMock = new Mock<IEntityRepository<IEntity>>(MockBehavior.Strict);
            repositoryMock.Setup(x => x.Find(StrippedRef.Id)).Returns(ResolvedRef);

            var factoryMock = new Mock<IEntityRepositoryFactory>(MockBehavior.Loose);
            factoryMock.Setup(x => x.Create(typeof(TRef))).Returns(repositoryMock.Object);

            var result = EntityWithStrippedRefs.ResolveReferences(factoryMock.Object);
            result.ShouldBeEqualTo(EntityWithResolvedRefs);

            factoryMock.Verify();
            repositoryMock.Verify();
        }

        [Test]
        public async void ResolveAsync()
        {
            var repositoryMock = new Mock<IEntityRepository<IEntity>>(MockBehavior.Strict);
            repositoryMock.Setup(x => x.FindAsync(StrippedRef.Id)).Returns(Task.FromResult((IEntity)ResolvedRef));

            var factoryMock = new Mock<IEntityRepositoryFactory>(MockBehavior.Loose);
            factoryMock.Setup(x => x.Create(typeof(TRef))).Returns(repositoryMock.Object);

            var result = await EntityWithStrippedRefs.ResolveReferencesAsync(factoryMock.Object);
            result.ShouldBeEqualTo(EntityWithResolvedRefs);

            factoryMock.Verify();
            repositoryMock.Verify();
        }

        [Test]
        public void Strip()
        {
            var result = EntityWithResolvedRefs.StripReferences();
            result.ShouldBeEqualTo(EntityWithStrippedRefs);
        }
    }
}