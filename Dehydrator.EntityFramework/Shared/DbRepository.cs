using System.Data.Entity;
using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace Dehydrator.EntityFramework
{
    /// <summary>
    /// Provides CRUD access to a set of <see cref="IEntity"/>s that is backed by a database accessed via Microsoft's Entity Framework.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of entities managed by this repository.</typeparam>
    public class DbRepository<TEntity> : DbReadRepository<TEntity>, IRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        [NotNull] private readonly DbContext _dbContext;
        [NotNull] private readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Creates a new database-backed repository.
        /// </summary>
        /// <param name="dbContext">The database context used to access the database.</param>
        /// <param name="dbSet">The database set used to store the entities.</param>
        public DbRepository(DbContext dbContext, DbSet<TEntity> dbSet)
            : base(dbSet)
        {
            _dbContext = dbContext;
            _dbSet = dbSet;
        }

        /// <summary>
        /// Creates a new database-backed repository.
        /// </summary>
        /// <param name="dbContext">The database context used to access the database.</param>
        public DbRepository(DbContext dbContext)
            : this(dbContext, dbContext.Set<TEntity>())
        {
        }

        public void Modify(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public TEntity Add(TEntity entity)
        {
            var storedEntity = _dbSet.Add(entity);
            _dbContext.SaveChanges();
            return storedEntity;
        }

        public bool Remove(int id)
        {
            var entity = Find(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }

#if NET45
        public Task<TEntity> FindAsync(int id)
        {
            return _dbSet.FindAsync(id);
        }

        public async Task ModifyAsync(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var storedEntity = _dbSet.Add(entity);
            await _dbContext.SaveChangesAsync();
            return storedEntity;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var entity = await FindAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
#endif
    }
}
