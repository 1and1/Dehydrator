using System.Net;
using System.Threading;
using System.Web.Http;
using FluentAssertions;

namespace Dehydrator.WebApi
{
    public static class HttpActionResultExtensions
    {
        public static void ShouldBe(this IHttpActionResult actionResult, HttpStatusCode statusCode)
        {
            actionResult.ExecuteAsync(CancellationToken.None).Result
                .StatusCode.Should().Be(statusCode);
        }
    }
}
