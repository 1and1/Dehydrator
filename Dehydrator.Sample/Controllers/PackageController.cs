using System.Web.Http;
using Dehydrator.Sample.Models;
using Dehydrator.WebApi;

namespace Dehydrator.Sample.Controllers
{
    [RoutePrefix("packages")]
    public class PackageController : AsyncCrudController<Package>
    {
        public PackageController(IRepository<Package> repository) : base(repository)
        {
        }
    }
}