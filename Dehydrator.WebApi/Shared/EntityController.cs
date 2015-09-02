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
        /// Checks whether the <see cref="IEntity.Id"/> matches <paramref name="id"/> and automatically sets <see cref="Entity.Id"/> if it is <see cref="Entity.NoId"/>.
        /// </summary>
        /// <exception cref="HttpResponseException">ID mismatch.</exception>
        protected void CheckIdInEntity(long id, TEntity entity)
        {
            if (entity.Id == Entity.NoId) entity.Id = id;
            else if (id != entity.Id)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    $"ID in URI ({id}) does not match ID in entity data ({entity.Id})."));
        }
    }
}
