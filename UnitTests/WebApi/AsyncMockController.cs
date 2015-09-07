namespace Dehydrator.WebApi
{
    public class AsyncMockController : AsyncCrudController<MockEntity1>
    {
        public AsyncMockController(ICrudRepository<MockEntity1> repository) : base(repository)
        {
        }
    }
}
