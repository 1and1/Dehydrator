using System.Data.Entity;
using JetBrains.Annotations;

namespace Dehydrator.EntityFramework
{
    /// <summary>
    /// A transaction with a table lock backed by an Entity Framework transaction.
    /// </summary>
    public sealed class DbTransaction : ITransaction
    {
        [NotNull] private readonly DbContextTransaction _transaction;
        private bool _commmited;

        public DbTransaction([NotNull] DbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Commit()
        {
            _commmited = true;
            _transaction.Commit();
        }

        /// <summary>
        /// Rolls back any changes that were made if <see cref="Commit"/> was not called and then releases the lock.
        /// </summary>
        public void Dispose()
        {
            if (!_commmited) _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}