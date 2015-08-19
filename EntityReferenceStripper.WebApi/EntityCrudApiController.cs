using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using JetBrains.Annotations;

namespace EntityReferenceStripper.WebApi
{
    [PublicAPI]
    public abstract class EntityCrudApiController<T> : ApiController
        where T : class, IEntity, new()
    {
        protected readonly DbContext Db;
        protected readonly IEntityResolver Resolver;

        protected EntityCrudApiController(DbContext db, IEntityResolver resolver)
        {
            Db = db;
            Resolver = resolver;
        }

        [HttpGet, Route("")]
        public IEnumerable<T> ReadAll()
        {
            return Db.Set<T>().Select(x => x.StripReferences());
        }

        [HttpGet, Route("{id}", Name = "bla")]
        public async Task<T> Read(int id)
        {
            var entity = await Db.Set<T>().FindAsync(id);
            if (entity == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    typeof (T).Name + " not found."));

            return entity.StripReferences();
        }

        [HttpPut, Route("{id}")]
        public async Task<IHttpActionResult> Update(int id, T entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != entity.Id)
            {
                return BadRequest();
            }

            var resolvedEntity = entity.ResolveReferences(Resolver);
            Db.Entry(resolvedEntity).State = EntityState.Modified;

            try
            {
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        private bool Exists(int id)
        {
            return Db.Set<T>().Any(e => e.Id == id);
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create(T entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resolvedEntity = entity.ResolveReferences(Resolver);
            var storedEntity = Db.Set<T>().Add(resolvedEntity);
            await Db.SaveChangesAsync();

            return Created(new Uri(resolvedEntity.Id.ToString(), UriKind.Relative), storedEntity);
            //return CreatedAtRoute("bla", new { id = resolvedEntity.Id }, storedEntity);
        }

        [HttpDelete, Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            T entity = await Db.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            Db.Set<T>().Remove(entity);
            await Db.SaveChangesAsync();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}