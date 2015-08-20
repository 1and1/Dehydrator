using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using JetBrains.Annotations;

namespace EntityReferenceStripper.WebApi
{
    /// <summary>
    /// Base class for building <see cref="ApiController"/>s that store entities in a <see cref="DbContext"/>.
    /// Wraps all access to the database with appropriate <see cref="EntityExtensions.StripReferences{T}"/> and <see cref="EntityExtensions.ResolveReferences{T}"/> calls.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of <see cref="IEntity"/> in use.</typeparam>
    [PublicAPI]
    public abstract class EntityApiController<TEntity> : ApiController
        where TEntity : class, IEntity, new()
    {
        [NotNull] private readonly DbContext _db;
        [NotNull] private readonly DbSet<TEntity> _dbSet;
        [NotNull] private readonly IEntityResolver _resolver;

        /// <summary>
        /// Instantiates a new Entity API controller.
        /// </summary>
        /// <param name="db">The database context used to store the entities.</param>
        /// <param name="resolver">The resolver used to map stripped <see cref="IEntity"/>s to resolved ones.</param>
        protected EntityApiController([NotNull] DbContext db, [NotNull] IEntityResolver resolver)
        {
            _db = db;
            _dbSet = _db.Set<TEntity>();
            _resolver = resolver;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Returns all entities from the backing database with stripped references.
        /// </summary>
        [NotNull]
        protected IEnumerable<TEntity> GetAll()
        {
            return _dbSet.Select(x => x.StripReferences());
        }

        /// <summary>
        /// Returns a specific entity from the backing database with stripped references.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to find.</param>
        /// <returns>The entity with stripped references or <see langword="null"/> if there was no match.</returns>
        [CanBeNull]
        protected TEntity Find(int id)
        {
            var entity = _dbSet.Find(id);
            return entity?.StripReferences();
        }

        /// <summary>
        /// Returns a specific entity from the backing database with stripped references.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to find.</param>
        /// <returns>The entity with stripped references or <see langword="null"/> if there was no match.</returns>
        protected async Task<TEntity> FindAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity?.StripReferences();
        }

        /// <summary>
        /// Modifies an existing entity in the database.
        /// </summary>
        /// <param name="entity">The modified entity with stripped resources.</param>
        protected void Modify([NotNull] TEntity entity)
        {
            var entityWithResolvedReferences = entity.ResolveReferences(_resolver);
            _db.Entry(entityWithResolvedReferences).State = EntityState.Modified;
            _db.SaveChanges();
        }

        /// <summary>
        /// Modifies an existing entity in the database.
        /// </summary>
        /// <param name="entity">The modified entity with stripped resources.</param>
        protected async Task ModifyAsync([NotNull] TEntity entity)
        {
            var entityWithResolvedReferences = entity.ResolveReferences(_resolver);
            _db.Entry(entityWithResolvedReferences).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Checks whether an entity with a given <see cref="IEntity.Id"/> exists in the database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to check.</param>
        protected bool Exists(int id)
        {
            return _dbSet.Any(e => e.Id == id);
        }

        /// <summary>
        /// Checks whether an entity with a given <see cref="IEntity.Id"/> exists in the database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to check.</param>
        protected Task<bool> ExistsAsync(int id)
        {
            return _dbSet.AnyAsync(e => e.Id == id);
        }

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <returns>The added entity with <see cref="IEntity.Id"/> set.</returns>
        [NotNull]
        protected TEntity Add([NotNull] TEntity entity)
        {
            var entityWithResolvedReferences = entity.ResolveReferences(_resolver);
            var storedEntity = _dbSet.Add(entityWithResolvedReferences);
            _db.SaveChanges();
            return storedEntity;
        }

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <returns>The added entity with <see cref="IEntity.Id"/> set.</returns>
        protected async Task<TEntity> AddAsync([NotNull] TEntity entity)
        {
            var entityWithResolvedReferences = entity.ResolveReferences(_resolver);
            var storedEntity = _dbSet.Add(entityWithResolvedReferences);
            await _db.SaveChangesAsync();
            return storedEntity;
        }

        /// <summary>
        /// Removes a specific entity from the database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to remove.</param>
        /// <returns><see langword="true"/> if the entity was removed; <see langword="false"/> if the entity did not exist.</returns>
        protected bool Remove(int id)
        {
            var entity = Find(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            _db.SaveChanges();
            return true;
        }

        /// <summary>
        /// Removes a specific entity from the database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to remove.</param>
        /// <returns><see langword="true"/> if the entity was removed; <see langword="false"/> if the entity did not exist.</returns>
        protected async Task<bool> RemoveAsync(int id)
        {
            var entity = await FindAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}