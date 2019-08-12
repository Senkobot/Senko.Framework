using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Senko.Commands.EfCore
{
    public class EfCoreRepository<TContext, TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        public EfCoreRepository(TContext context)
        {
            Context = context;
            Set = context.Set<TEntity>();
        }

        protected DbSet<TEntity> Set { get; }

        protected TContext Context { get; }

        public void Add(TEntity entity)
        {
            Context.Add(entity);
        }

        public void Update(TEntity entity)
        {
            Context.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            Context.Remove(entity);
        }

        public Task SaveChangesAsync(CancellationToken token = default)
        {
            return Context.SaveChangesAsync(token);
        }
    }
}
