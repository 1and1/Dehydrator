using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Dehydrator.WebApi
{
    /// <summary>
    /// A generic base for REST controllers that operate on a specific <see cref="IEntity"/> type.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of entities accessible via this controller.</typeparam>
    public abstract class EntityController<TEntity> : ApiController
        where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// Throws an exception if <paramref name="entity"/> is <see langword="null"/>. Returns the <paramref name="entity"/> otherwise.
        /// </summary>
        /// <exception cref="HttpResponseException"><paramref name="entity"/> is <see langword="null"/>.</exception>
        protected TEntity CheckFound(TEntity entity, long id)
        {
            if (entity == null)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    $"{typeof(TEntity).Name} {id} not found."));
            return entity;
        }

        /// <summary>
        /// Checks whether the <see cref="IEntity.Id"/> matches <paramref name="id"/> and automatically sets <see cref="Entity.Id"/> if it is <see cref="Entity.NoId"/>.
        /// </summary>
        /// <exception cref="HttpResponseException">ID mismatch.</exception>
        protected void CheckIdInEntity(TEntity entity, long id)
        {
            if (entity.Id == Entity.NoId) entity.Id = id;
            else if (id != entity.Id)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    $"ID in URI ({id}) does not match ID in entity data ({entity.Id})."));
        }
    }
}
