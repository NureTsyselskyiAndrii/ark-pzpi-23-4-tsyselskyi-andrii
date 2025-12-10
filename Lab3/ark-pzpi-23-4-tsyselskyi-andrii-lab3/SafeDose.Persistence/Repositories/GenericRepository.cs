using Microsoft.EntityFrameworkCore;
using SafeDose.Application.Contracts.Persistence;
using SafeDose.Domain.Common;
using SafeDose.Persistence.DbContexts;

namespace SafeDose.Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey> :
    IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        protected readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TEntity>> GetAsync()
        {
            return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(q => q.Id!.Equals(id));
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(TKey key)
        {
            var trackedEntity = await _context.Set<TEntity>().FindAsync(key);
            if (trackedEntity is not null)
            {
                _context.Remove(trackedEntity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
