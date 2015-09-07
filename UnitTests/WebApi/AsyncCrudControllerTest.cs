using System.Net.Http;
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
        private Mock<IRepository<MockEntity1>> _repositoryMock;
        private AsyncMockController _controller;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IRepository<MockEntity1>>(MockBehavior.Strict);

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
            _repositoryMock.Setup(x => x.FindUntypedAsync(entity.Id))
                .Returns(Task.FromResult<IEntity>(entity));
            (await _controller.Read(entity.Id)).Should().Be(entity);
        }

        [Test]
        public void TestReadNotFound()
        {
            _repositoryMock.Setup(x => x.FindUntypedAsync(1)).Returns(Task.FromResult<IEntity>(null));
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
            _repositoryMock.Setup(x => x.Add(entity)).Returns(entity);
            _repositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
            await _controller.Create(entity);
        }
    }
}