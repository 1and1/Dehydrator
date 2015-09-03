namespace Dehydrator.WebApi
{
    public class MockController : CrudController<MockEntity1>
    {
        public MockController(IRepository<MockEntity1> repository) : base(repository)
        {
        }
    }
}
