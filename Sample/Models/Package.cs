using System.Collections.Generic;
using Dehydrator;

namespace DehydratorSample.Models
{
    /// <summary>
    /// A software package like an application or a library.
    /// </summary>
    public class Package : NamedEntity
    {
        /// <summary>
        /// An old package this one replaces.
        /// </summary>
        [Dehydrate]
        public virtual Package Replaces { get; set; }

        /// <summary>
        /// All dependencies this package depends on.
        /// </summary>
        [Dehydrate]
        public virtual ICollection<Package> Dependencies { get; set; }

        /// <summary>
        /// All packages that list this packages in <seealso cref="Dependencies"/>.
        /// </summary>
        [Dehydrate]
        public virtual ICollection<Package> DependencyOf { get; set; }

        /// <summary>
        /// A set of key-value configuration entries.
        /// </summary>
        public virtual ICollection<PackageConfig> Configs { get; set; }
    }
}