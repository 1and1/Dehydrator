using System.Data.Entity;

namespace Dehydrator.EntityFramework
{
    public class MockDbContext : DbContext
    {
        public DbSet<MockEntity1> MockEntity1s { get; set; }
        public DbSet<MockEntity2> MockEntity2s { get; set; }
    }
}
