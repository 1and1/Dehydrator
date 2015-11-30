using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Dehydrator.WebApi
{
    [TestFixture]
    public class AsyncCrudControllerTest
    {
        private Mock<ICrudRepository<MockEntity1>> _repositoryMock;
        private AsyncMockController _controller;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<ICrudRepository<MockEntity1>>(MockBehavior.Strict);

            _controller = new AsyncMockController(_repositoryMock.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        [TearDown]
        public void TearDown()
        {
            _repositoryMock.VerifyAll();
        }

        [Test]
        public async Task TestRead()
        {
            var entity = new MockEntity1 {Id = 1, FriendlyName = "Mock"};
            _repositoryMock.Setup(x => x.FindAsync(entity.Id, CancellationToken.None))
                .ReturnsAsync(entity);
            (await _controller.Read(entity.Id)).Should().Be(entity);
        }

        [Test]
        public void TestReadNotFound()
        {
            _repositoryMock.Setup(x => x.FindAsync(1, CancellationToken.None))
                .Returns(Task.FromResult<MockEntity1>(null));
            Assert.Throws<HttpResponseException>(async () => await _controller.Read(1));
        }

        [Test]
        public void TestReadAll()
        {
            var entity = new MockEntity1 {Id = 1, FriendlyName = "Mock"};
            _repositoryMock.Setup(x => x.GetAll()).Returns(new[] {entity});
            _controller.ReadAll().Should().Equal(entity);
        }

        [Test]
        public async Task TestCreate()
        {
            var entity = new MockEntity1 {Id = 1, FriendlyName = "Mock"};
            _repositoryMock.Setup(x => x.AddAsync(entity, CancellationToken.None))
                .ReturnsAsync(entity);
            await _controller.Create(entity);
        }

        [Test]
        public async Task TestCreateResolveFailed()
        {
            var entity = new MockEntity1 {Id = 1, FriendlyName = "Mock"};
            _repositoryMock.Setup(x => x.AddAsync(entity, CancellationToken.None)).Throws<KeyNotFoundException>();
            (await _controller.Create(entity)).ShouldBe(HttpStatusCode.BadRequest);
        }

        [Test]
        public void TestCreateInvalidData()
        {
            var entity = new MockEntity1 {Id = 1, FriendlyName = "Mock"};
            _repositoryMock.Setup(x => x.AddAsync(entity, CancellationToken.None)).Throws<DataException>();
            Assert.Throws<HttpResponseException>(async () => await _controller.Create(entity));
        }

        [Test]
        public async Task TestUpdate()
        {
            var entity = new MockEntity1 {Id = 1, FriendlyName = "Mock"};
            _repositoryMock.Setup(x => x.ModifyAsync(entity, CancellationToken.None)).ReturnsAsync();
            await _controller.Update(1, entity);
        }

        [Test]
        public async Task TestDelete()
        {
            _repositoryMock.Setup(x => x.RemoveAsync(1, CancellationToken.None)).ReturnsAsync(true);
            await _controller.Delete(1);
        }

        [Test]
        public void TestDeleteMissing()
        {
            _repositoryMock.Setup(x => x.RemoveAsync(1, CancellationToken.None)).ReturnsAsync(false);
            Assert.Throws<HttpResponseException>(async () => await _controller.Delete(1));
        }
    }
}