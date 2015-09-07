using System.Data.Entity;
using JetBrains.Annotations;

namespace Dehydrator.EntityFramework
{
    /// <summary>
    /// Provides <see cref="ICrudRepository{T}"/>s that are backed by a database accessed via Microsoft's Entity Framework.
    /// </summary>
    [PublicAPI]
    public class DbRepositoryFactory : DbReadRepositoryFactory, ICrudRepositoryFactory
    {
        /// <summary>
        /// Creates a new database-backed entity repository factory.
        /// </summary>
        /// <param name="dbContext">The database context used to access the database.</param>
        public DbRepositoryFactory([NotNull] DbContext dbContext)
            : base(dbContext)
        {
        }

        public new ICrudRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return new DbCrudRepository<TEntity>(DbContext.Set<TEntity>(), DbContext);
        }
    }
}
