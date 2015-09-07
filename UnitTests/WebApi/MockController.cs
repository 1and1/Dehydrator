namespace Dehydrator.WebApi
{
    public class MockController : CrudController<MockEntity1>
    {
        public MockController(ICrudRepository<MockEntity1> repository) : base(repository)
        {
        }
    }
}
