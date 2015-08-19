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
        private static readonly MockEntity _strippedRef = new MockEntity { Id = 2 };
        private static readonly MockEntity _entityWithStrippedReference = new MockEntity { Id = 1, Reference = _strippedRef };
        private static readonly MockEntity _resolvedRef = new MockEntity { Id = 2, Resolved = true };
        private static readonly MockEntity _entityWithResolvedReference = new MockEntity { Id = 1, Reference = _resolvedRef };

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
            _entityResolverMock.Setup(x => x.Resolve(_strippedRef, typeof (MockEntity))).Returns(_resolvedRef);
            _dbSetMock.Setup(x => x.Add(_entityWithResolvedReference)).Returns(_entityWithResolvedReference).Verifiable();
            _dbContextMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(1)).Verifiable();

            var result = await _controller.Create(_entityWithStrippedReference);
            var response = await result.ExecuteAsync(CancellationToken.None);

            response.IsSuccessStatusCode.ShouldBeTrue();

        }

        [Test]
        public async void TestRead()
        {
            _dbSetMock.Setup(x => x.FindAsync(1)).Returns(Task.FromResult(_entityWithResolvedReference)).Verifiable();

            var result = await _controller.Read(1);
            result.ShouldBeEqualTo(_entityWithStrippedReference);
        }

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
                if (obj.GetType() != this.GetType()) return false;
                return Equals((MockEntity) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = base.GetHashCode();
                    hashCode = (hashCode*397) ^ (Reference != null ? Reference.GetHashCode() : 0);
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