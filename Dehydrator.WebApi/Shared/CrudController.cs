using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using JetBrains.Annotations;

namespace Dehydrator.WebApi
{
    /// <summary>
    /// A generic base for REST controllers that provide CRUD access to a set of entities exposed via an <see cref="IRepository{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of entities accessible via this controller.</typeparam>
    [PublicAPI]
    public abstract class CrudController<TEntity> : ReadController<TEntity>
        where TEntity : class, IEntity, new()
    {
        [NotNull] protected new readonly IRepository<TEntity> Repository;

        protected CrudController([NotNull] IRepository<TEntity> repository)
            : base(repository)
        {
            Repository = repository;
        }

        [HttpPost, Route("")]
        public virtual IHttpActionResult Create([CanBeNull] TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (entity == null) return BadRequest("Missing request body.");

            TEntity storedEntity;
            try
            {
                storedEntity = Repository.Add(entity);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }

            return Created(
                location: new Uri(storedEntity.Id.ToString(), UriKind.Relative),
                content: storedEntity.DehydrateReferences());
        }

        [HttpPut, Route("{id}")]
        public virtual IHttpActionResult Update(long id, [CanBeNull] TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (entity == null) return BadRequest("Missing request body.");
            if (id != entity.Id) return BadRequest($"ID in URI ({id}) does not match ID in entity data ({entity.Id}).");

            try
            {
                Repository.Modify(entity);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete, Route("{id}")]
        public virtual void Delete(long id)
        {
            if (!Repository.Remove(id))
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    $"{typeof(TEntity).Name} {id} not found."));
        }
    }
}