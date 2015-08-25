using System.Collections.Generic;

namespace Dehydrator.Sample.Models
{
    /// <summary>
    /// A software package like an application or a library.
    /// </summary>
    public class Package : Entity
    {
        public string FriendlyName { get; set; }

        /// <summary>
        /// An old package this one replaces.
        /// </summary>
        public virtual Package Replaces { get; set; }

        /// <summary>
        /// All dependencies this package depends on.
        /// </summary>
        public virtual ICollection<Package> Dependencies { get; set; }

        /// <summary>
        /// All packages that list this packages in <seealso cref="Dependencies"/>.
        /// </summary>
        public virtual ICollection<Package> DependencyOf { get; set; }
    }
}