using System;
using System.Data.Entity;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssert;
using Moq;
using NUnit.Framework;

namespace EntityReferenceStripper.WebApi
{
    [TestFixture]
    public class EntityCrudApiControllerTest
    {
        private static readonly MockEntity StrippedRef = new MockEntity {Id = 2};
        private static readonly MockEntity EntityWithStrippedReference = new MockEntity {Id = 1, Reference = StrippedRef};
        private static readonly MockEntity ResolvedRef = new MockEntity {Id = 2, Resolved = true};
        private static readonly MockEntity EntityWithResolvedReference = new MockEntity {Id = 1, Reference = ResolvedRef};

        private Mock<DbSet<MockEntity>> _dbSetMock;
        private Mock<DbContext> _dbContextMock;
        private Mock<IEntityResolver> _entityResolverMock;
        private EntityCrudApiController<MockEntity> _controller;

        [SetUp]
        public void SetUp()
        {
            _dbSetMock = new Mock<DbSet<MockEntity>>(MockBehavior.Strict);

            _dbContextMock = new Mock<DbContext>(MockBehavior.Strict);
            _dbContextMock.Setup(x => x.Set<MockEntity>()).Returns(_dbSetMock.Object);

            _entityResolverMock = new Mock<IEntityResolver>(MockBehavior.Strict);

            _controller = new MockController(_dbContextMock.Object, _entityResolverMock.Object)
            {
                Request = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost/api/products")
                },
                Configuration = new HttpConfiguration()
            };
        }

        [TearDown]
        public void TearDown()
        {
            _dbSetMock.Verify();
            _dbContextMock.Verify();
            _entityResolverMock.Verify();
        }

        [Test]
        public async void TestCreate()
        {
            _entityResolverMock.Setup(x => x.Resolve(StrippedRef, typeof (MockEntity))).Returns(ResolvedRef);
            _dbSetMock.Setup(x => x.Add(EntityWithResolvedReference)).Returns(EntityWithResolvedReference).Verifiable();
            _dbContextMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(1)).Verifiable();

            var result = await _controller.Create(EntityWithStrippedReference);
            var response = await result.ExecuteAsync(CancellationToken.None);

            response.IsSuccessStatusCode.ShouldBeTrue();
        }

        [Test]
        public async void TestRead()
        {
            _dbSetMock.Setup(x => x.FindAsync(1)).Returns(Task.FromResult(EntityWithResolvedReference)).Verifiable();

            var result = await _controller.Read(1);
            result.ShouldBeEqualTo(EntityWithStrippedReference);
        }

        [Test]
        public async void TestDelete()
        {
            _dbSetMock.Setup(x => x.FindAsync(1)).Returns(Task.FromResult(EntityWithResolvedReference)).Verifiable();
            _dbSetMock.Setup(x => x.Remove(EntityWithStrippedReference)).Returns(EntityWithResolvedReference).Verifiable();
            _dbContextMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(1)).Verifiable();

            var result = await _controller.Delete(1);
            var response = await result.ExecuteAsync(CancellationToken.None);

            response.IsSuccessStatusCode.ShouldBeTrue();
        }

        // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
        public class MockEntity : Entity
        {
            public virtual MockEntity Reference { get; set; }
            public bool Resolved { get; set; }

            #region Equality

            protected bool Equals(MockEntity other)
            {
                return base.Equals(other) && Equals(Reference, other.Reference) && Resolved == other.Resolved;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((MockEntity) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = base.GetHashCode();
                    hashCode = (hashCode*397) ^ (Reference?.GetHashCode() ?? 0);
                    hashCode = (hashCode*397) ^ Resolved.GetHashCode();
                    return hashCode;
                }
            }

            #endregion
        }

        private class MockController : EntityCrudApiController<MockEntity>
        {
            public MockController(DbContext db, IEntityResolver resolver) : base(db, resolver)
            {
            }
        }
    }
}