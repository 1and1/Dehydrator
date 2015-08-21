using System.Data.Entity;
using JetBrains.Annotations;

namespace Dehydrator.EntityFramework
{
    /// <summary>
    /// Provides <see cref="IRepository{T}"/>s that are backed by a database accessed via Microsoft's Entity Framework.
    /// </summary>
    [PublicAPI]
    public class DbRepositoryFactory : IRepositoryFactory
    {
        [NotNull] private readonly DbContext _dbContext;

        /// <summary>
        /// Creates a new database-backed entity repository factory.
        /// </summary>
        /// <param name="dbContext">The database context used to access the database.</param>
        public DbRepositoryFactory([NotNull] DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return new DbRepository<TEntity>(_dbContext);
        }
    }
}
