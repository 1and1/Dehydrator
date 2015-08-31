using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Dehydrator.EntityFramework
{
    internal static class ContextExtensions
    {
        /// <summary>
        /// Returns the name of the database table associated with entities of the type <typeparamref name="T"/>.
        /// </summary>
        [NotNull]
        public static string GetTableName<T>([NotNull] this DbContext context)
            where T : class
        {
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            string sql = objectContext.CreateObjectSet<T>().ToTraceString();

            var match = new Regex("FROM (?<table>.*) AS").Match(sql);
            return match.Groups["table"].Value;
        }
    }
}
