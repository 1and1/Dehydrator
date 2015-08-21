using System;
using System.Data.Entity;
using JetBrains.Annotations;

namespace Dehydrator.WebApi
{
    /// <summary>
    /// Provides <see cref="IEntityRepository{T}"/>s that are backed by a database accessed via Microsoft's Entity Framework.
    /// </summary>
    [PublicAPI]
    public class DbEntityRepositoryFactory : IEntityRepositoryFactory
    {
        [NotNull] private readonly DbContext _db;

        /// <summary>
        /// Creates a new database-backed entity repository factory.
        /// </summary>
        /// <param name="db">The database context used to store the entities.</param>>
        public DbEntityRepositoryFactory([NotNull] DbContext db)
        {
            _db = db;
        }

        public IEntityRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return new DbEntityRepository<TEntity>(_db);
        }

        public IEntityRepository<IEntity> Create(Type entityType)
        {
            return new DbEntityRepository(_db, entityType);
        }
    }
}
