using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Dehydrator
{
    public static class RepositoryFactoryExtensions
    {
        private static readonly MethodInfo CreateReadMethod =
            typeof(IReadRepositoryFactory).GetMethod(nameof(IReadRepositoryFactory.Create));

        /// <summary>
        /// Returns a read-only repository for a specific <paramref name="entityType"/>.
        /// </summary>
        [NotNull]
        public static object Create([NotNull] this IReadRepositoryFactory repositoryFactory,
            [NotNull] Type entityType)
        {
            return CreateReadMethod.MakeGenericMethod(entityType).Invoke(repositoryFactory, null);
        }

        private static readonly MethodInfo CreateCrudMethod =
            typeof(ICrudRepositoryFactory).GetMethod(nameof(ICrudRepositoryFactory.Create));

        /// <summary>
        /// Returns a CRUD repository for a specific <paramref name="entityType"/>.
        /// </summary>
        [NotNull]
        public static object Create([NotNull] this ICrudRepositoryFactory repositoryFactory,
            [NotNull] Type entityType)
        {
            return CreateCrudMethod.MakeGenericMethod(entityType).Invoke(repositoryFactory, null);
        }
    }
}