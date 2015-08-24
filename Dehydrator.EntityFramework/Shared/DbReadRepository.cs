using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace Dehydrator.EntityFramework
{
    /// <summary>
    /// Provides read-only access to a set of <see cref="IEntity"/>s that is backed by a database accessed via Microsoft's Entity Framework.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of entities managed by this repository.</typeparam>
    public class DbReadRepository<TEntity> : IReadRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        [NotNull] private readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Creates a new database-backed repository.
        /// </summary>
        /// <param name="dbSet">The database set used to store the entities.</param>
        public DbReadRepository(DbSet<TEntity> dbSet)
        {
            _dbSet = dbSet;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet;
        }

        public bool Exists(long id)
        {
            return _dbSet.Any(e => e.Id == id);
        }

        public TEntity Find(long id)
        {
            return _dbSet.Find(id);
        }

#if NET45
        public async Task<IEntity> FindUntypedAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }
#endif
    }
}
