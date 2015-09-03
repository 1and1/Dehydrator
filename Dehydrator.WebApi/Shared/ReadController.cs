using System.Collections.Generic;
using System.Web.Http;
using JetBrains.Annotations;

namespace Dehydrator.WebApi
{
    /// <summary>
    /// A generic base for REST controllers that provide read-only access to a set of entities exposed via an <see cref="IReadRepository{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of entities accessible via this controller.</typeparam>
    [PublicAPI]
    public abstract class ReadController<TEntity> : EntityController<TEntity>
        where TEntity : class, IEntity, new()
    {
        [NotNull] protected readonly IReadRepository<TEntity> Repository;

        protected ReadController([NotNull] IReadRepository<TEntity> repository)
        {
            Repository = repository;
        }

        [HttpGet, Route("")]
        public virtual IEnumerable<TEntity> ReadAll()
        {
            return Repository.GetAll();
        }

        [HttpGet, Route("{id}")]
        public virtual TEntity Read(long id)
        {
            return CheckFound(Repository.Find(id), id);
        }
    }
}