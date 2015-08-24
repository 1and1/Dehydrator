﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
    public abstract class AsyncCrudController<TEntity> : ApiController
        where TEntity : class, IEntity, new()
    {
        [NotNull] protected readonly IRepository<TEntity> Repository;

        protected AsyncCrudController([NotNull] IRepository<TEntity> repository)
        {
            Repository = repository;
        }

        [HttpGet, Route("")]
        public IEnumerable<TEntity> ReadAll()
        {
            return Repository.GetAll();
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create(TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var storedEntity = await Repository.AddAsync(entity);
            return Created(new Uri(storedEntity.Id.ToString(), UriKind.Relative), storedEntity.DehydrateReferences());
        }

        [HttpGet, Route("{id}", Name = "bla")]
        public async Task<TEntity> Read(long id)
        {
            var entity = await Repository.FindAsync(id);
            if (entity == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    typeof(TEntity).Name + " not found."));
            return entity;
        }

        [HttpPut, Route("{id}")]
        public async Task<IHttpActionResult> Update(long id, TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != entity.Id) return BadRequest("ID in URI does not match ID in Entity data.");

            await Repository.ModifyAsync(entity);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete, Route("{id}")]
        public async Task<IHttpActionResult> Delete(long id)
        {
            if (await Repository.RemoveAsync(id)) return StatusCode(HttpStatusCode.NoContent);
            else return NotFound();
        }
    }
}