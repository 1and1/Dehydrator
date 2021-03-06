﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dehydrator.EntityFramework;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Dehydrator
{
    [TestFixture]
    public class DehydratingCrudRepositoryTest
    {
        private Mock<ICrudRepository<MockEntity1>> _mainRepositoryMock;
        private Mock<ICrudRepository<MockEntity2>> _refRepositoryMock;
        private DehydratingCrudRepository<MockEntity1> _repository;

        private MockEntity1 _entityWithDehydratedRefs, _entityWithResolvedRefs;
        private MockEntity2 _resolvedRef, _dehydratedRef;
        private MockEntity1 _dehydratedEntity;

        [SetUp]
        public void SetUp()
        {
            _mainRepositoryMock = new Mock<ICrudRepository<MockEntity1>>();
            _refRepositoryMock = new Mock<ICrudRepository<MockEntity2>>();
            var factory = new LookupCrudRepositoryFactory {_refRepositoryMock.Object};
            _repository = new DehydratingCrudRepository<MockEntity1>(_mainRepositoryMock.Object, factory);

            _dehydratedRef = new MockEntity2 {Id = 2};
            _entityWithDehydratedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                MultiRef = {_dehydratedRef}
            };

            _resolvedRef = new MockEntity2 {Id = 2, FriendlyName = "Bar"};
            _entityWithResolvedRefs = new MockEntity1
            {
                Id = 1,
                FriendlyName = "Foo",
                MultiRef = {_resolvedRef}
            };

            _dehydratedEntity = new MockEntity1 {Id = 1};
        }

        [TearDown]
        public void TearDown()
        {
            _mainRepositoryMock.VerifyAll();
            _refRepositoryMock.VerifyAll();
        }

        [Test]
        public void TestGetAll()
        {
            _mainRepositoryMock.Setup(x => x.GetAll())
                .Returns(new[] {_entityWithResolvedRefs});

            var result = _repository.GetAll().ToList();
            result.Should().BeEquivalentTo(_dehydratedEntity);
        }

        [Test]
        public void TestGetAllPredicate()
        {
            _mainRepositoryMock.Setup(x => x.GetAll(_ => true))
                .Returns(new[] {_entityWithResolvedRefs});

            var result = _repository.GetAll(_ => true);
            result.Should().BeEquivalentTo(_dehydratedEntity);
        }

        [Test]
        public void TestFind()
        {
            _mainRepositoryMock.Setup(x => x.Find(_entityWithDehydratedRefs.Id))
                .Returns(_entityWithResolvedRefs);

            var result = _repository.Find(_entityWithDehydratedRefs.Id);
            result.Should().Be(_entityWithDehydratedRefs);
        }

        [Test]
        public async Task TestFindAsync()
        {
            _mainRepositoryMock.Setup(x => x.FindAsync(_entityWithDehydratedRefs.Id, CancellationToken.None))
                .ReturnsAsync(_entityWithResolvedRefs);

            var result = await _repository.FindAsync(_entityWithDehydratedRefs.Id);
            result.Should().Be(_entityWithDehydratedRefs);
        }

        [Test]
        public void TestExists()
        {
            _mainRepositoryMock.Setup(x => x.Exists(123))
                .Returns(true);

            _repository.Exists(123).Should().BeTrue();
        }

        [Test]
        public void TestQueryable()
        {
            _mainRepositoryMock.SetupGet(x => x.Query)
                .Returns(new DbQueryable<MockEntity1>(new[] {_entityWithResolvedRefs}.AsQueryable()));

            _repository.Query.Should().Equal(_entityWithDehydratedRefs);
            _repository.Query.First().Should().Be(_entityWithDehydratedRefs);
        }

        [Test]
        public void TestAdd()
        {
            _refRepositoryMock.Setup(x => x.Find(_dehydratedRef.Id))
                .Returns(_resolvedRef);
            _mainRepositoryMock.Setup(x => x.Add(_entityWithResolvedRefs))
                .Returns(_entityWithResolvedRefs);

            var result = _repository.Add(_entityWithDehydratedRefs);
            result.Should().Be(_entityWithDehydratedRefs);
        }

        [Test]
        public void TestModify()
        {
            _refRepositoryMock.Setup(x => x.Find(_dehydratedRef.Id))
                .Returns(_resolvedRef);
            _mainRepositoryMock.Setup(x => x.Modify(_entityWithResolvedRefs));

            _repository.Modify(_entityWithDehydratedRefs);
        }

        [Test]
        public async Task TestModifyAsync()
        {
            _refRepositoryMock.Setup(x => x.FindAsync(_dehydratedRef.Id, CancellationToken.None))
                .ReturnsAsync(_resolvedRef);
            _mainRepositoryMock.Setup(x => x.ModifyAsync(_entityWithResolvedRefs, CancellationToken.None))
                .ReturnsAsync();

            await _repository.ModifyAsync(_entityWithDehydratedRefs);
        }

        [Test]
        public void TestRemove()
        {
            _mainRepositoryMock.Setup(x => x.Remove(123))
                .Returns(true);

            _repository.Remove(123).Should().BeTrue();
        }

        [Test]
        public async Task TestRemoveAsync()
        {
            _mainRepositoryMock.Setup(x => x.RemoveAsync(123, CancellationToken.None))
                .ReturnsAsync(true);

            (await _repository.RemoveAsync(123)).Should().BeTrue();
        }
    }
}