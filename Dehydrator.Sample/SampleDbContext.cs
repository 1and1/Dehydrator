using System.Data.Entity;
using Dehydrator.Sample.Models;

namespace Dehydrator.Sample
{
    public class SampleDbContext : DbContext
    {
        public DbSet<Package> Resources { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Package>()
                .HasMany(x => x.Dependencies)
                .WithMany(x => x.DependencyOf)
                .Map(x =>
                {
                    x.MapLeftKey("Dependant");
                    x.MapRightKey("Dependency");
                    x.ToTable("DependencyRelations");
                });
        }

        public SampleDbContext() : base("Dehydrator.Sample")
        {}
    }
}
