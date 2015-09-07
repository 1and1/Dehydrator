using System.Net;
using System.Net.Http;
using System.Web.Http;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Dehydrator.WebApi
{
    [TestFixture]
    public class CrudControllerTest
    {
        private Mock<ICrudRepository<MockEntity1>> _repositoryMock;
        private MockController _controller;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<ICrudRepository<MockEntity1>>(MockBehavior.Strict);

            _controller = new MockController(_repositoryMock.Object)
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
        public void TestRead()
        {
            var entity = new MockEntity1 {Id = 1, FriendlyName = "Mock"};
            _repositoryMock.Setup(x => x.Find(entity.Id)).Returns(entity);
            _controller.Read(entity.Id).Should().Be(entity);
        }

        [Test]
        public void TestReadNotFound()
        {
            _repositoryMock.Setup(x => x.Find(1)).Returns<MockEntity1>(null);
            _controller.Invoking(x => x.Read(1))
                .ShouldThrow<HttpResponseException>().Where(x => x.Response.StatusCode == HttpStatusCode.NotFound);
        }

        [Test]
        public void TestReadAll()
        {
            var entity = new MockEntity1 {Id = 1, FriendlyName = "Mock"};
            _repositoryMock.Setup(x => x.GetAll()).Returns(new[] {entity});
            _controller.ReadAll().Should().Equal(entity);
        }

        [Test]
        public void TestCreate()
        {
            var entity = new MockEntity1 {Id = 1, FriendlyName = "Mock"};
            _repositoryMock.Setup(x => x.Add(entity)).Returns(entity);
            _repositoryMock.Setup(x => x.SaveChanges());
            _controller.Create(entity);
        }
    }
}