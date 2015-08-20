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

        private Mock<DbContext> _dbContextMock;
        private Mock<DbSet<MockEntity>> _dbGenericSetMock;
        private Mock<DbSet> _dbUntypedSetMock;

        private EntityCrudApiController<MockEntity> _controller;

        [SetUp]
        public void SetUp()
        {
            _dbContextMock = new Mock<DbContext>(MockBehavior.Strict);

            _dbGenericSetMock = new Mock<DbSet<MockEntity>>(MockBehavior.Strict);
            _dbContextMock.Setup(x => x.Set<MockEntity>()).Returns(_dbGenericSetMock.Object);

            _dbUntypedSetMock = new Mock<DbSet>(MockBehavior.Strict);
            _dbContextMock.Setup(x => x.Set(typeof(MockEntity))).Returns(_dbUntypedSetMock.Object);

            _controller = new MockController(_dbContextMock.Object)
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
            _dbContextMock.Verify();
            _dbGenericSetMock.Verify();
            _dbUntypedSetMock.Verify();
        }

        [Test]
        public async void TestCreate()
        {
            _dbUntypedSetMock.Setup(x => x.Find(StrippedRef.Id)).Returns(ResolvedRef);
            _dbGenericSetMock.Setup(x => x.Add(EntityWithResolvedReference)).Returns(EntityWithResolvedReference).Verifiable();
            _dbContextMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(1)).Verifiable();

            var result = await _controller.Create(EntityWithStrippedReference);
            var response = await result.ExecuteAsync(CancellationToken.None);

            response.IsSuccessStatusCode.ShouldBeTrue();
        }

        [Test]
        public async void TestRead()
        {
            _dbGenericSetMock.Setup(x => x.FindAsync(1)).Returns(Task.FromResult(EntityWithResolvedReference)).Verifiable();

            var result = await _controller.Read(1);
            result.ShouldBeEqualTo(EntityWithStrippedReference);
        }

        [Test]
        public async void TestDelete()
        {
            _dbGenericSetMock.Setup(x => x.FindAsync(1)).Returns(Task.FromResult(EntityWithResolvedReference)).Verifiable();
            _dbGenericSetMock.Setup(x => x.Remove(EntityWithStrippedReference)).Returns(EntityWithResolvedReference).Verifiable();
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
            public MockController(DbContext db) : base(db)
            {
            }
        }
    }
}