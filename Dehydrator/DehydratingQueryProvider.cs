using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Dehydrator
{
    /// <summary>
    /// Wraps an <see cref="IQueryProvider"/> and runs <see cref="DehydrationUtils.DehydrateReferences{T}"/> on the results.
    /// </summary>
    /// <seealso cref="DehydratingQueryable"/>
    /// <seealso cref="DehydratingQueryable{T}"/>
    internal class DehydratingQueryProvider : IQueryProvider
    {
        private readonly IQueryProvider _inner;

        public DehydratingQueryProvider([NotNull] IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new DehydratingQueryable(_inner.CreateQuery(expression));
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new DehydratingQueryable<TElement>(_inner.CreateQuery<TElement>(expression));
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression).DehydrateReferences();
        }
    }
}