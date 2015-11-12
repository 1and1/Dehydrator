using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Dehydrator
{
    [TestFixture]
    public class QueryableExtensionsTest
    {
        [Test]
        public async Task ToList()
        {
            var elements = new[] {1, 2, 3}.AsQueryable();
            (await elements.ToListAsync()).Should().Equal(1, 2, 3);
        }

        [Test]
        public async Task First()
        {
            var elements = new[] {1, 2, 3}.AsQueryable();
            (await elements.FirstAsync()).Should().Be(1);
        }

        [Test]
        public async Task FirstOrDefault()
        {
            var elements = new[] {1, 2, 3}.AsQueryable();
            (await elements.FirstOrDefaultAsync()).Should().Be(1);
        }

        [Test]
        public async Task FirstOrDefaultEmpty()
        {
            var elements = Enumerable.Empty<object>().AsQueryable();
            (await elements.FirstOrDefaultAsync()).Should().Be(null);
        }

        [Test]
        public async Task ToListAsync()
        {
            var mock = new Mock<IAsyncQueryable<int>>();
            mock.Setup(x => x.ToListAsync(CancellationToken.None)).Returns(Task.FromResult(new List<int> {1, 2, 3}));
            (await mock.Object.ToListAsync()).Should().Equal(1, 2, 3);
        }

        [Test]
        public async Task FirstAsync()
        {
            var mock = new Mock<IAsyncQueryable<int>>();
            mock.Setup(x => x.FirstAsync(CancellationToken.None)).Returns(Task.FromResult(1));
            (await mock.Object.FirstAsync()).Should().Be(1);
        }

        [Test]
        public async Task FirstOrDefaultAsync()
        {
            var mock = new Mock<IAsyncQueryable<int>>();
            mock.Setup(x => x.FirstOrDefaultAsync(CancellationToken.None)).Returns(Task.FromResult(1));
            (await mock.Object.FirstOrDefaultAsync()).Should().Be(1);
        }
    }
}