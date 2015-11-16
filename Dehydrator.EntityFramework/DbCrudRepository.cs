using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading;
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
        [NotNull] internal readonly DbContext DbContext;
        [NotNull] private readonly DbSet<TEntity> _dbSet;

        private bool _transactionActive;

        /// <summary>
        /// Creates a new database-backed repository.
        /// </summary>
        /// <param name="dbSet">The database set used to store the entities.</param>
        /// <param name="dbContext">The database context used to access the database.</param>
        public DbCrudRepository(DbSet<TEntity> dbSet, DbContext dbContext)
            : base(dbSet)
        {
            DbContext = dbContext;
            _dbSet = dbSet;
        }

        public TEntity Add(TEntity entity)
        {
            var result = _dbSet.Add(entity);

            DbContext.SaveChanges();
            return result;
        }

        public void Modify(TEntity entity)
        {
            if (!IsTracked(entity))
            {
                var existingEntity = Find(entity.Id);
                if (existingEntity == null)
                    throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {entity.Id} not found.");
                entity.TransferState(to: existingEntity);
            }

            DbContext.SaveChanges();
        }

        /// <summary>
        /// Determines whether <paramref name="entity"/> is wrapped in a tracking proxy and can therefore detect changes to itself.
        /// </summary>
        private bool IsTracked(TEntity entity)
        {
            return DbContext.ChangeTracker.Entries<TEntity>().Any(x => x.Entity == entity);
        }

        public bool Remove(long id)
        {
            var entity = Find(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);

            DbContext.SaveChanges();
            return true;
        }

        public ITransaction BeginTransaction()
        {
            if (_transactionActive) return new FakeTransaction();

            var transaction = DbContext.Database.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                DbContext.Database.ExecuteSqlCommand(
                    $"SELECT 1 FROM {DbContext.GetTableName<TEntity>()} WITH (TABLOCKX, HOLDLOCK)");
            }
            catch
            {
                transaction.Dispose();
                throw;
            }

            _transactionActive = true;
            return new DbTransaction(transaction, disposeCallback: () => _transactionActive = false);
        }

#if NET45
        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = _dbSet.Add(entity);

            await DbContext.SaveChangesAsync(cancellationToken);
            return result;
        }

        public async Task ModifyAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!IsTracked(entity))
            {
                var existingEntity = await FindAsync(entity.Id, cancellationToken);
                if (existingEntity == null)
                    throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {entity.Id} not found.");
                entity.TransferState(to: existingEntity);
            }

            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> RemoveAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entity = await FindAsync(id, cancellationToken);
            if (entity == null) return false;

            _dbSet.Remove(entity);

            await DbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_transactionActive) return new FakeTransaction();

            var transaction = DbContext.Database.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                await
                    DbContext.Database.ExecuteSqlCommandAsync(
                        $"SELECT 1 FROM {DbContext.GetTableName<TEntity>()} WITH (TABLOCKX, HOLDLOCK)", cancellationToken);
            }
            catch
            {
                transaction.Dispose();
                throw;
            }

            _transactionActive = true;
            return new DbTransaction(transaction, disposeCallback: () => _transactionActive = false);
        }
#endif
    }
}