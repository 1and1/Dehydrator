﻿using System;
using System.Collections;
using System.Collections.Generic;
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

        public TEntity Add(TEntity entity)
        {
            var storedEntity = _dbSet.Add(entity);
            _dbContext.SaveChanges();
            return storedEntity;
        }

        public void Modify(TEntity entity)
        {
            // NOTE: _dbContext.Entry(entity).State = EntityState.Modified; will not work here due to references
            var existingEntity = Find(entity.Id);
            if (existingEntity == null)
                throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {entity.Id} not found.");
            TransferState(from: entity, to: existingEntity);
            _dbContext.SaveChanges();
        }

        public bool Remove(long id)
        {
            var entity = Find(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// Copies all public properties <paramref name="from"/> one <see cref="IEntity"/> <paramref name="to"/> another. Special handling for <see cref="IEntity"/> collections.
        /// </summary>
        private static void TransferState([NotNull] TEntity from, [NotNull] TEntity to)
        {
            var entityType = typeof(TEntity);

            foreach (var prop in entityType.GetWritableProperties())
            {
                var fromValue = prop.GetValue(from, null);
                if (prop.IsEntityCollection())
                {
                    var referenceType = prop.GetGenericArg();

                    object targetList = prop.GetValue(to, null);
                    Type collectionType;
                    if (targetList == null)
                    {
                        collectionType = typeof(List<>).MakeGenericType(referenceType);
                        targetList = Activator.CreateInstance(collectionType);
                        prop.SetValue(obj: to, value: targetList, index: null);
                    }
                    else
                    {
                        collectionType = targetList.GetType();
                        collectionType.InvokeClear(target: targetList);
                    }

                    if (fromValue == null) continue;
                    foreach (IEntity reference in (IEnumerable)fromValue)
                    {
                        collectionType.InvokeAdd(target: targetList,
                            value: reference);
                    }
                }
                else prop.SetValue(obj: to, value: fromValue, index: null);
            }
        }

#if NET45
        public Task<TEntity> FindAsync(long id)
        {
            return _dbSet.FindAsync(id);
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var storedEntity = _dbSet.Add(entity);
            await _dbContext.SaveChangesAsync();
            return storedEntity;
        }

        public async Task ModifyAsync(TEntity entity)
        {
            // NOTE: _dbContext.Entry(entity).State = EntityState.Modified; will not work here due to references
            var existingEntity = await FindAsync(entity.Id);
            if (existingEntity == null)
                throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {entity.Id} not found.");
            TransferState(from: entity, to: existingEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(long id)
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