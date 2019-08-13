using Microsoft.EntityFrameworkCore;
using Senko.Commands.EfCore;

namespace Senko.Commands.Tests.Data
{
    public class TestDbContext : DbContext
    {
        public TestDbContext()
        { }

        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder model)
        {
            base.OnModelCreating(model);

            model.AddCommand();
        }
    }
}
