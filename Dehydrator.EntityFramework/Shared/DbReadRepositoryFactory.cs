using System.Data.Entity;
using JetBrains.Annotations;

namespace Dehydrator.EntityFramework
{
    /// <summary>
    /// Provides <see cref="IReadRepository{T}"/>s that are backed by a database accessed via Microsoft's Entity Framework.
    /// </summary>
    [PublicAPI]
    public class DbReadRepositoryFactory : IReadRepositoryFactory
    {
        [NotNull] protected readonly DbContext DbContext;

        /// <summary>
        /// Creates a new database-backed entity repository factory.
        /// </summary>
        /// <param name="dbContext">The database context used to access the database.</param>
        public DbReadRepositoryFactory([NotNull] DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IReadRepository<TEntity> Create<TEntity>()
            where TEntity : class, IEntity, new()
        {
            return new DbReadRepository<TEntity>(DbContext.Set<TEntity>());
        }
    }
}