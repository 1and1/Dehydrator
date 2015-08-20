using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using JetBrains.Annotations;

namespace EntityReferenceStripper.WebApi
{
    [PublicAPI]
    public abstract class EntityCrudApiController<TEntity> : EntityApiController<TEntity>
        where TEntity : class, IEntity, new()
    {
        protected EntityCrudApiController([NotNull] DbContext db) : base(db)
        {
        }

        [HttpGet, Route("")]
        public IEnumerable<TEntity> ReadAll()
        {
            return GetAll();
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create(TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var storedEntity = await AddAsync(entity);

            return Created(new Uri(storedEntity.Id.ToString(), UriKind.Relative), storedEntity.StripReferences());
            //return CreatedAtRoute("bla", new { id = resolvedEntity.Id }, storedEntity);
        }

        [HttpGet, Route("{id}", Name = "bla")]
        public async Task<TEntity> Read(int id)
        {
            var entity = await FindAsync(id);
            if (entity == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, typeof(TEntity).Name + " not found."));

            return entity;
        }

        [HttpPut, Route("{id}")]
        public async Task<IHttpActionResult> Update(int id, TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != entity.Id) return BadRequest("ID in URI does not match ID in Entity data.");

            try
            {
                await ModifyAsync(entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await ExistsAsync(id))) return NotFound();
                else throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete, Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (await RemoveAsync(id)) return StatusCode(HttpStatusCode.NoContent);
            else return NotFound();
        }
    }
}