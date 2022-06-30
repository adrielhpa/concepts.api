using Concepts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Concepts.Repository
{
    public class ConceptContext : DbContext
    {
        public ConceptContext(DbContextOptions<ConceptContext> opt): base(opt){}

        public DbSet<User> Users{ get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}