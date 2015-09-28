using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using JetBrains.Annotations;

namespace Dehydrator.WebApi
{
    /// <summary>
    /// A generic base for REST controllers that provide CRUD access to a set of entities exposed via an <see cref="ICrudRepository{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of entities accessible via this controller.</typeparam>
    [PublicAPI]
    public abstract class CrudController<TEntity> : ReadController<TEntity>
        where TEntity : class, IEntity, new()
    {
        [NotNull] protected new readonly ICrudRepository<TEntity> Repository;

        protected CrudController([NotNull] ICrudRepository<TEntity> repository)
            : base(repository)
        {
            Repository = repository;
        }

        /// <summary>
        /// Creates a new <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="entity">The new <typeparamref name="TEntity"/>.</param>
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

            SaveChanges();
            return Created(
                location: new Uri(storedEntity.Id.ToString(), UriKind.Relative),
                content: storedEntity.DehydrateReferences());
        }

        /// <summary>
        /// Updates an existing <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to update.</param>
        /// <param name="entity">The modified <typeparamref name="TEntity"/>.</param>
        [HttpPut, Route("{id}")]
        public virtual IHttpActionResult Update(long id, [CanBeNull] TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (entity == null) return BadRequest("Missing request body.");
            CheckIdInEntity(entity, id);

            try
            {
                Repository.Modify(entity);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }

            SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Deletes an existing <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to delete.</param>
        [HttpDelete, Route("{id}")]
        public virtual void Delete(long id)
        {
            if (!Repository.Remove(id))
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    $"{typeof(TEntity).Name} {id} not found."));

            SaveChanges(codeIfError: HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Saves any pending changes in the underlying <see cref="Repository"/>.
        /// </summary>
        /// <param name="codeIfError">The <see cref="HttpStatusCode"/> to return in case of an error.</param>
        protected void SaveChanges(HttpStatusCode codeIfError = HttpStatusCode.BadRequest)
        {
            try
            {
                Repository.SaveChanges();
            }
            catch (DataException ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(codeIfError, ex.GetInnerMost().Message));
            }
        }
    }
}