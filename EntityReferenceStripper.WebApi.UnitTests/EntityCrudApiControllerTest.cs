using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

        private Mock<DbContext> _dbMock;
        private Mock<DbSet<MockEntity>> _dbSetGenericMock;
        private Mock<DbSet> _dbSetUntypedMock;

        private MockController _controller;

        [SetUp]
        public void SetUp()
        {
            _dbMock = new Mock<DbContext>(MockBehavior.Strict);

            _dbSetGenericMock = new Mock<DbSet<MockEntity>>(MockBehavior.Strict);
            _dbMock.Setup(x => x.Set<MockEntity>()).Returns(_dbSetGenericMock.Object);

            _dbSetUntypedMock = new Mock<DbSet>(MockBehavior.Strict);
            _dbMock.Setup(x => x.Set(typeof(MockEntity))).Returns(_dbSetUntypedMock.Object);

            _controller = new MockController(_dbMock.Object)
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
            _dbMock.Verify();
            _dbSetGenericMock.Verify();
            _dbSetUntypedMock.Verify();
        }

        [Test]
        public async void TestCreate()
        {
            _dbSetUntypedMock.Setup(x => x.FindAsync(StrippedRef.Id)).Returns(Task.FromResult((object)ResolvedRef));
            _dbSetGenericMock.Setup(x => x.Add(EntityWithResolvedReference)).Returns(EntityWithResolvedReference).Verifiable();
            _dbMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(1)).Verifiable();

            var result = await _controller.Create(EntityWithStrippedReference);
            var response = await result.ExecuteAsync(CancellationToken.None);

            response.IsSuccessStatusCode.ShouldBeTrue();
        }

        [Test]
        public async void TestRead()
        {
            _dbSetGenericMock.Setup(x => x.FindAsync(1)).Returns(Task.FromResult(EntityWithResolvedReference)).Verifiable();

            var result = await _controller.Read(1);
            result.ShouldBeEqualTo(EntityWithStrippedReference);
        }

        [Test]
        public async void TestDelete()
        {
            _dbSetGenericMock.Setup(x => x.FindAsync(1)).Returns(Task.FromResult(EntityWithResolvedReference)).Verifiable();
            _dbSetGenericMock.Setup(x => x.Remove(EntityWithStrippedReference)).Returns(EntityWithResolvedReference).Verifiable();
            _dbMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(1)).Verifiable();

            var result = await _controller.Delete(1);
            var response = await result.ExecuteAsync(CancellationToken.None);

            response.IsSuccessStatusCode.ShouldBeTrue();
        }

        private class MockController : EntityCrudApiController<MockEntity>
        {
            public MockController(DbContext db) : base(db)
            {
            }
        }
    }
}