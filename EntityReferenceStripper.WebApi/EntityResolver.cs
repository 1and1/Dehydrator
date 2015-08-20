using System;
using System.Data.Entity;
using JetBrains.Annotations;

namespace EntityReferenceStripper.WebApi
{
    /// <summary>
    /// Resolves incomplete <see cref="IEntity"/>s that only specify <see cref="IEntity.Id"/> to full entities using a <see cref="DbContext"/>.
    /// </summary>
    public class EntityResolver : IEntityResolver
    {
        [NotNull] private readonly DbContext _db;

        /// <summary>
        /// Creates a new entity resolver.
        /// </summary>
        /// <param name="db">The database to retrieve entities from.</param>
        public EntityResolver([NotNull] DbContext db)
        {
            _db = db;
        }

        public IEntity Resolve(IEntity entity, Type entityType)
        {
            return (IEntity)_db.Set(entityType).Find(entity.Id);
        }
    }
}
