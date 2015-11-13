﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using JetBrains.Annotations;

#if NET45
using System.Threading.Tasks;
#endif

namespace Dehydrator
{
    /// <summary>
    /// Provides read-only access to a set of <see cref="IEntity"/>s. Usually backed by a database.
    /// </summary>
    public interface IReadRepository<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Returns all entities of a certain type from the backing database.
        /// </summary>
        /// <remarks>Results may be dehydrated.</remarks>
        [NotNull]
        [DataObjectMethod(DataObjectMethodType.Select)]
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Returns all entities of a certain type that match a predicate.
        /// </summary>
        /// <remarks>Results may be dehydrated.</remarks>
        [NotNull]
        IEnumerable<TEntity> GetAll([NotNull] Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Checks whether an entity with a given <see cref="IEntity.Id"/> exists in the database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to check.</param>
        bool Exists(long id);

        /// <summary>
        /// Returns a specific entity from the backing database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to find.</param>
        /// <returns>The entity or <see langword="null"/> if there was no match.</returns>
        /// <remarks>Result references may be dehydrated.</remarks>
        [CanBeNull]
        TEntity Find(long id);

        /// <summary>
        /// Provides LINQ access to the underlying data.
        /// </summary>
        /// <remarks>Result references may be dehydrated.</remarks>
        IQueryable<TEntity> Query { get; }

#if NET45
        /// <summary>
        /// Returns a specific entity from the backing database.
        /// </summary>
        /// <param name="id">The <see cref="IEntity.Id"/> of the entity to find.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The entity or <see langword="null"/> if there was no match.</returns>
        /// <remarks>Result references may be dehydrated.</remarks>
        Task<TEntity> FindAsync(long id, CancellationToken cancellationToken = default(CancellationToken));
#endif
    }
}