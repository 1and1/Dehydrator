namespace Dehydrator.WebApi
{
    public class AsyncMockController : AsyncCrudController<MockEntity1>
    {
        public AsyncMockController(IRepository<MockEntity1> repository) : base(repository)
        {
        }
    }
}
