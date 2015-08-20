using System;
using JetBrains.Annotations;

namespace EntityReferenceStripper
{
    /// <summary>
    /// Resolves incomplete <see cref="IEntity"/>s that only specify <see cref="IEntity.Id"/> to full entities.
    /// </summary>
    public interface IEntityResolver
    {
        /// <summary>
        /// Resolves an <see cref="IEntity"/> that has been stripped to contain nothing but its <see cref="IEntity.Id"/> and returns the full entity.
        /// </summary>
        /// <param name="entity">The entity to resolve.</param>
        /// <param name="entityType">The concrete type of <paramref name="entity"/>.</param>
        [Pure, NotNull]
        IEntity Resolve([NotNull] IEntity entity, [NotNull] Type entityType);
    }
}