using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Dehydrator.EntityFramework
{
    /// <summary>
    /// Wraps an <see cref="IQueryProvider"/> for <see cref="DbQueryable"/>.
    /// </summary>
    /// <seealso cref="DbQueryable"/>
    /// <seealso cref="DbQueryable{T}"/>
    internal class DbQueryProvider : IQueryProvider
    {
        private readonly IQueryProvider _inner;

        public DbQueryProvider([NotNull] IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new DbQueryable(_inner.CreateQuery(expression));
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new DbQueryable<TElement>(_inner.CreateQuery<TElement>(expression));
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }
    }
}