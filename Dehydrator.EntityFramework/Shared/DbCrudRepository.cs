using System.Collections.Generic;
using System.Data;
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
    public class DbCrudRepository<TEntity> : DbReadRepository<TEntity>, ICrudRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        [NotNull] private readonly DbContext _dbContext;
        [NotNull] private readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Creates a new database-backed repository.
        /// </summary>
        /// <param name="dbSet">The database set used to store the entities.</param>
        /// <param name="dbContext">The database context used to access the database.</param>
        public DbCrudRepository(DbSet<TEntity> dbSet, DbContext dbContext)
            : base(dbSet)
        {
            _dbContext = dbContext;
            _dbSet = dbSet;
        }

        public TEntity Add(TEntity entity)
        {
            return _dbSet.Add(entity);
        }

        /// <summary>
        /// Modifies an existing entity in the database.
        /// </summary>
        /// <param name="entity">The modified entity. If any <see cref="IEntity"/> collections are <see langword="null"/> they are treated as unmodified rather than empty.</param>
        /// <exception cref="KeyNotFoundException">No existing entity with matching <see cref="IEntity.Id"/> in the backing database.</exception>
        public void Modify(TEntity entity)
        {
            // NOTE: _dbContext.Entry(entity).State = EntityState.Modified; will not work here due to references
            var existingEntity = Find(entity.Id);
            if (existingEntity == null)
                throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {entity.Id} not found.");
            entity.TransferState(to: existingEntity);
        }

        public bool Remove(long id)
        {
            var entity = Find(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            return true;
        }

        public ITransaction BeginTransaction()
        {
            var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                _dbContext.Database.ExecuteSqlCommand(
                    $"SELECT 1 FROM {_dbContext.GetTableName<TEntity>()} WITH (TABLOCKX, HOLDLOCK)");
            }
            catch
            {
                transaction.Dispose();
                throw;
            }
            return new DbTransaction(transaction);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

#if NET45
        public Task<TEntity> FindAsync(long id)
        {
            return _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Modifies an existing entity in the database.
        /// </summary>
        /// <param name="entity">The modified entity. If any <see cref="IEntity"/> collections are <see langword="null"/> they are treated as unmodified rather than empty.</param>
        /// <exception cref="KeyNotFoundException">No existing entity with matching <see cref="IEntity.Id"/> in the backing database.</exception>
        public async Task ModifyAsync(TEntity entity)
        {
            // NOTE: _dbContext.Entry(entity).State = EntityState.Modified; will not work here due to references
            var existingEntity = await FindAsync(entity.Id);
            if (existingEntity == null)
                throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {entity.Id} not found.");
            entity.TransferState(to: existingEntity);
        }

        public async Task<bool> RemoveAsync(long id)
        {
            var entity = await FindAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            return true;
        }

        public async Task<ITransaction> BeginTransactionAsync()
        {
            var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                await
                    _dbContext.Database.ExecuteSqlCommandAsync(
                        $"SELECT 1 FROM {_dbContext.GetTableName<TEntity>()} WITH (TABLOCKX, HOLDLOCK)");
            }
            catch
            {
                transaction.Dispose();
                throw;
            }
            return new DbTransaction(transaction);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
#endif
    }
}