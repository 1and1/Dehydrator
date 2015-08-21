using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using JetBrains.Annotations;

namespace Dehydrator.WebApi
{
    /// <summary>
    /// A generic REST controller that provides CRUD access to a set of entities exposed via an <see cref="IEntityRepository{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of entities accessible via this controller.</typeparam>
    [PublicAPI]
    public abstract class EntityCrudApiController<TEntity> : ApiController
        where TEntity : class, IEntity, new()
    {
        [NotNull] private readonly IEntityRepository<TEntity> _repository;

        protected EntityCrudApiController([NotNull] IEntityRepository<TEntity> repository)
        {
            _repository = repository;
        }

        [HttpGet, Route("")]
        public IEnumerable<TEntity> ReadAll()
        {
            return _repository.GetAll();
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create(TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var storedEntity = await _repository.AddAsync(entity);
            return Created(new Uri(storedEntity.Id.ToString(), UriKind.Relative), storedEntity.DehydrateReferences());
        }

        [HttpGet, Route("{id}", Name = "bla")]
        public async Task<TEntity> Read(int id)
        {
            var entity = await _repository.FindAsync(id);
            if (entity == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    typeof (TEntity).Name + " not found."));
            return entity;
        }

        [HttpPut, Route("{id}")]
        public async Task<IHttpActionResult> Update(int id, TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != entity.Id) return BadRequest("ID in URI does not match ID in Entity data.");

            await _repository.ModifyAsync(entity);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete, Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (await _repository.RemoveAsync(id)) return StatusCode(HttpStatusCode.NoContent);
            else return NotFound();
        }
    }
}
