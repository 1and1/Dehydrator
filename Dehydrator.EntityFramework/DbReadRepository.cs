using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

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
        [NotNull] private readonly IQueryable<TEntity> _queryable;

        /// <summary>
        /// Creates a new database-backed repository.
        /// </summary>
        /// <param name="dbSet">The database set used to store the entities.</param>
        public DbReadRepository(DbSet<TEntity> dbSet)
        {
            _dbSet = dbSet;
            _queryable = dbSet;
        }

        public IEnumerable<TEntity> GetAll() => _dbSet.AsNoTracking();

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate) => _dbSet.AsNoTracking().Where(predicate);

        public bool Exists(long id) => _dbSet.Any(x => x.Id == id);

        public TEntity Find(long id) => _queryable.FirstOrDefault(x => x.Id == id);

        public IQueryable<TEntity> Query => new DbQueryable<TEntity>(_queryable);

        public async Task<TEntity> FindAsync(long id, CancellationToken cancellationToken = default(CancellationToken)) => await _queryable.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
