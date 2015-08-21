using System;
using System.Data.Entity;
using JetBrains.Annotations;

namespace Dehydrator.WebApi
{
    /// <summary>
    /// Provides <see cref="IRepository{T}"/>s that are backed by a database accessed via Microsoft's Entity Framework.
    /// </summary>
    [PublicAPI]
    public class DbRepositoryFactory : IRepositoryFactory
    {
        [NotNull] private readonly DbContext _db;

        /// <summary>
        /// Creates a new database-backed entity repository factory.
        /// </summary>
        /// <param name="db">The database context used to store the entities.</param>>
        public DbRepositoryFactory([NotNull] DbContext db)
        {
            _db = db;
        }

        public IRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return new DbRepository<TEntity>(_db);
        }

        public IRepository<IEntity> Create(Type entityType)
        {
            return new DbRepository(_db, entityType);
        }
    }
}
