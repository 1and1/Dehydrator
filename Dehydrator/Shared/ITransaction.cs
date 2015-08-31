using System;

namespace Dehydrator
{
    /// <summary>
    /// Represents a transaction started by a specific <see cref="IRepository{TEntity}"/>.
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// Releases the transaction lock and commits any changes.
        /// </summary>
        void Commit();
    }
}