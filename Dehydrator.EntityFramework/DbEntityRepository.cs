using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace Dehydrator.WebApi
{
    /// <summary>
    /// Provides CRUD access to a set of <see cref="IEntity"/>s that is backed by a database accessed via Microsoft's Entity Framework.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of entities managed by this repository.</typeparam>
    internal class DbEntityRepository<TEntity> : IEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        [NotNull] private readonly DbContext _db;
        [NotNull] private readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Creates a new database-backed repository.
        /// </summary>
        /// <param name="db">The database context used to store the entities.</param>>
        public DbEntityRepository(DbContext db)
        {
            _db = db;
            _dbSet = db.Set<TEntity>();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet;
        }

        public bool Exists(int id)
        {
            return _dbSet.Any(e => e.Id == id);
        }

        public TEntity Find(int id)
        {
            return _dbSet.Find(id);
        }

        public void Modify(TEntity entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public TEntity Add(TEntity entity)
        {
            var storedEntity = _dbSet.Add(entity);
            _db.SaveChanges();
            return storedEntity;
        }

        public bool Remove(int id)
        {
            var entity = Find(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            _db.SaveChanges();
            return true;
        }

#if NET45
        public Task<TEntity> FindAsync(int id)
        {
            return _dbSet.FindAsync(id);
        }

        public async Task ModifyAsync(TEntity entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var storedEntity = _dbSet.Add(entity);
            await _db.SaveChangesAsync();
            return storedEntity;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var entity = await FindAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
#endif
    }

    /// <summary>
    /// Provides CRUD access to a set of <see cref="IEntity"/>s that is backed by a database accessed via Microsoft's Entity Framework.
    /// </summary>
    internal class DbEntityRepository : IEntityRepository<IEntity>
    {
        [NotNull] private readonly DbContext _db;
        [NotNull] private readonly DbSet _dbSet;

        /// <summary>
        /// Creates a new database-backed repository.
        /// </summary>
        /// <param name="db">The database context used to store the entities.</param>
        /// <param name="entityType">The specific type of entities managed by this repository.</param>
        /// >
        public DbEntityRepository(DbContext db, Type entityType)
        {
            _db = db;
            _dbSet = db.Set(entityType);
        }

        public IEnumerable<IEntity> GetAll()
        {
            return _dbSet.Cast<IEntity>();
        }

        public bool Exists(int id)
        {
            return _dbSet.Cast<IEntity>().Any(e => e.Id == id);
        }

        public IEntity Find(int id)
        {
            return (IEntity)_dbSet.Find(id);
        }

        public void Modify(IEntity entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public IEntity Add(IEntity entity)
        {
            var storedEntity = _dbSet.Add(entity);
            _db.SaveChanges();
            return (IEntity)storedEntity;
        }

        public bool Remove(int id)
        {
            var entity = Find(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            _db.SaveChanges();
            return true;
        }

#if NET45
        public async Task<IEntity> FindAsync(int id)
        {
            var result = await _dbSet.FindAsync(id);
            return (IEntity)result;
        }

        public async Task ModifyAsync(IEntity entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<IEntity> AddAsync(IEntity entity)
        {
            var storedEntity = _dbSet.Add(entity);
            await _db.SaveChangesAsync();
            return (IEntity)storedEntity;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var entity = await FindAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
#endif
    }
}
