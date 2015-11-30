using System.Threading.Tasks;
using Moq;
using Moq.Language;
using Moq.Language.Flow;

namespace Dehydrator
{
    /// <summary>
    /// Provides extension methods for <see cref="Mock{T}"/>s.
    /// </summary>
    public static class MockExtensions
    {
        /// <summary>
        /// Configures the mock to return a completed <see cref="Task"/>.
        /// </summary>
        public static IReturnsResult<TMock> ReturnsAsync<TMock>(this IReturns<TMock, Task> mock)
            where TMock : class
        {
            return mock.Returns(Task.FromResult<object>(null));
        }
    }
}