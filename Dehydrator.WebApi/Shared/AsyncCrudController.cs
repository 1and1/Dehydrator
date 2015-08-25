using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using JetBrains.Annotations;

namespace Dehydrator.WebApi
{
    /// <summary>
    /// A generic base for REST controllers that provide CRUD access to a set of entities exposed via an <see cref="IRepository{TEntity}"/>. Uses asynchronous processing.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of entities accessible via this controller.</typeparam>
    [PublicAPI]
    public abstract class AsyncCrudController<TEntity> : AsyncReadController<TEntity>
        where TEntity : class, IEntity, new()
    {
        [NotNull] protected new readonly IRepository<TEntity> Repository;

        protected AsyncCrudController([NotNull] IRepository<TEntity> repository)
            : base(repository)
        {
            Repository = repository;
        }

        [HttpPost, Route("")]
        public virtual async Task<IHttpActionResult> Create(TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var storedEntity = await Repository.AddAsync(entity);
            return Created(
                location: new Uri(storedEntity.Id.ToString(), UriKind.Relative),
                content: storedEntity.DehydrateReferences());
        }

        [HttpPut, Route("{id}")]
        public virtual async Task<IHttpActionResult> Update(long id, TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != entity.Id) return BadRequest($"ID in URI ({id}) does not match ID in entity data ({entity.Id}).");

            try
            {
                await Repository.ModifyAsync(entity);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete, Route("{id}")]
        public virtual async Task<IHttpActionResult> Delete(long id)
        {
            if (await Repository.RemoveAsync(id)) return StatusCode(HttpStatusCode.NoContent);
            else return NotFound();
        }
    }
}