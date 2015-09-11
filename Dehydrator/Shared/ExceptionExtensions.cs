using System;

namespace Dehydrator
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Follows the chain of <see cref="Exception.InnerException"/>s to the end (turtles all the way down).
        /// </summary>
        public static Exception GetInnerMost(this Exception ex)
        {
            return ex.InnerException?.GetInnerMost() ?? ex;
        }
    }
}
