using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Dehydrator;
using Dehydrator.WebApi;
using DehydratorSample.Models;

namespace DehydratorSample.Controllers
{
    [RoutePrefix("packages")]
    public class PackageController : AsyncCrudController<Package>
    {
        public PackageController(ICrudRepository<Package> repository) : base(repository)
        {
        }

        [HttpPost, Route("test-data")]
        public async Task TestData()
        {
            await Repository.AddAsync(new Package
            {
                Name = "AwesomeApp",
                Dependencies = new List<Package>
                {
                    new Package
                    {
                        Name = "AwesomeLib"
                    }
                }
            });
        }
    }
}