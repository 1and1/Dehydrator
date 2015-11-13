using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Dehydrator.WebApi
{
    /// <summary>
    /// Provides pagination support via the HTTP Range header.
    /// </summary>
    public static class HttpPagination
    {
        /// <summary>
        /// The value used for <see cref="RangeHeaderValue.Unit"/>.
        /// </summary>
        public const string RangeUnit = "elements";

        /// <summary>
        /// Generates a response from the <paramref name="queryable"/> and applies any pagination specified in the <paramref name="request"/>.
        /// </summary>
        [NotNull]
        public static async Task<HttpResponseMessage> CreatePagedResponseAsync<T>(
            [NotNull] this HttpRequestMessage request,
            [NotNull] IOrderedQueryable<T> queryable)
        {
            if (request.Headers.Range?.Unit != RangeUnit)
                return request.CreateResponse(queryable);

            var range = request.Headers.Range.Ranges.First();

            IReadOnlyCollection<T> elements;
            long from;
            if (range.From.HasValue)
            {
                from = range.From.Value;
                elements = range.To.HasValue
                    ? await queryable.SubsetAsync(range.From.Value, range.To.Value)
                    : await queryable.SkipAsync(range.From.Value);
            }
            else if (range.To.HasValue)
                elements = await queryable.TailAsync(range.To.Value, out from);
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    "Range must specify upper or lower bound or both.");
            }

            if (elements.Count == 0)
            {
                return request.CreateErrorResponse(HttpStatusCode.RequestedRangeNotSatisfiable,
                    "No elements in requested range at this time. Try again later.");
            }
            else
            {
                var response = request.CreateResponse(HttpStatusCode.PartialContent, elements);
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(from, from + elements.Count - 1)
                {
                    Unit = RangeUnit
                };
                return response;
            }
        }

        /// <summary>
        /// Retrieves a subset of all elements in a query.
        /// </summary>
        /// <param name="queryable">The data source.</param>
        /// <param name="from">The index of the first element to retrieve (inclusive).</param>
        /// <param name="to">The index of the last element to retrieve (inclusive).</param>
        [NotNull]
        private static async Task<IReadOnlyCollection<T>> SubsetAsync<T>([NotNull] this IQueryable<T> queryable,
            long from, long to)
        {
            return await queryable.Skip((int)from).Take((int)(to - from + 1)).ToListAsync();
        }

        /// <summary>
        /// Retrieves all elements in a query skipping a specific number of elements at the start. Performs long polling if the result set is empty.
        /// </summary>
        /// <param name="queryable">The data source.</param>
        /// <param name="from">The index of the first element to retrieve (inclusive).</param>
        [NotNull]
        private static async Task<IReadOnlyCollection<T>> SkipAsync<T>([NotNull] this IQueryable<T> queryable, long from)
        {
            return await queryable.Skip((int)from).LongPollAsync();
        }

        /// <summary>
        /// Controls how many query attempts are performed for a long poll before giving up.
        /// </summary>
        public static int LongPollingRounds = 10;

        /// <summary>
        /// Controls how many milliseconds to wait between query attempts for a long poll.
        /// </summary>
        public static int LongPollingDelay = 1500;

        /// <summary>
        /// Tries to retrieve all elements in a query. Performs long polling if the result set is empty.
        /// </summary>
        /// <param name="queryable">The data source.</param>
        [NotNull]
        private static async Task<IReadOnlyCollection<T>> LongPollAsync<T>([NotNull] this IQueryable<T> queryable)
        {
            for (int i = 0; i < LongPollingRounds; i++)
            {
                var result = await queryable.ToListAsync();
                if (result.Count > 0) return result;
                await Task.Delay(LongPollingDelay);
            }
            return new List<T>();
        }

        /// <summary>
        /// Retrieves the last n elements in a query.
        /// </summary>
        /// <param name="queryable">The data source.</param>
        /// <param name="tail">The number of elements to retrieve.</param>
        /// <param name="from">Returns the index of the first retrieved element (inclusive).</param>
        [NotNull]
        private static Task<IReadOnlyCollection<T>> TailAsync<T>([NotNull] this IQueryable<T> queryable, long tail,
            out long from)
        {
            from = queryable.LongCount() - tail;
            return queryable.SubsetAsync(from, from + tail);
        }
    }
}